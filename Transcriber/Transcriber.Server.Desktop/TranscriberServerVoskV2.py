import json
import queue
import socket
import threading

from vosk import Model, KaldiRecognizer


class SingleClientAudioServer:
    def __init__(self, model_path, audio_port=9999, result_port=9998, sample_rate=16000):
        self.sample_rate = sample_rate
        self.audio_queue = queue.Queue()

        # Загружаем модель Vosk
        print(f"Загрузка модели Vosk из {model_path}...")
        self.model = Model(model_path)
        self.rec = KaldiRecognizer(self.model, self.sample_rate)

        # Параметры сокетов
        self.audio_port = audio_port
        self.result_port = result_port

        # Сокеты и флаги
        self.audio_socket = None
        self.result_socket = None
        self.audio_client = None
        self.result_client = None
        self.running = False
        self.processing_active = False

    def audio_processing_worker(self):
        """Рабочий поток для обработки аудиоданных"""
        print("Запущен поток обработки аудио")

        self.rec.Reset()
        self.processing_active = True

        while self.processing_active:
            try:
                audio_bytes = self.audio_queue.get(timeout=0.5)

                if self.rec.AcceptWaveform(audio_bytes):
                    result = json.loads(self.rec.Result())
                    text = result.get("text", "")
                    if text:
                        print(f"Распознано: {text}")

                        self._send_result({
                            "type": "final",
                            "text": text
                        })
                else:
                    partial = json.loads(self.rec.PartialResult())
                    partial_text = partial.get("partial", "")

                    if partial_text:
                        self._send_result({
                            "type": "partial",
                            "text": partial_text
                        })

                self.audio_queue.task_done()

            except queue.Empty:
                if not self.processing_active:
                    break
                continue
            except Exception as e:
                print(f"Ошибка при обработке аудио: {e}")
                continue

        print("Поток обработки аудио завершен")

    def _send_result(self, result_data):
        """Отправка результата клиенту"""
        if not self.result_client:
            return

        try:
            result_json = json.dumps(result_data) + "\n"
            self.result_client.sendall(result_json.encode('utf-8'))
        except Exception as e:
            print(f"Ошибка при отправке результата: {e}")
            self._close_result_connection()

    def _close_result_connection(self):
        """Закрытие соединения для результатов"""
        if self.result_client:
            try:
                self.result_client.close()
            except:
                pass
            self.result_client = None
            print("Соединение для результатов закрыто")

    def _close_audio_connection(self):
        """Закрытие соединения для аудио"""
        if self.audio_client:
            try:
                self.audio_client.close()
            except:
                pass
            self.audio_client = None
            print("Соединение для аудио закрыто")

    def handle_audio_connection(self):
        """Обработчик соединения для аудио"""
        print("Ожидание подключения для аудио...")

        self.audio_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.audio_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

        try:
            self.audio_socket.bind(('0.0.0.0', self.audio_port))
            self.audio_socket.listen(1)
            self.audio_socket.settimeout(1.0)

            while self.running:
                try:
                    client_socket, client_address = self.audio_socket.accept()
                    client_socket.settimeout(0.5)

                    print(f"Подключен клиент аудио: {client_address}")

                    if self.audio_client:
                        self._close_audio_connection()

                    self.audio_client = client_socket

                    while self.running and self.audio_client:
                        try:
                            data = self.audio_client.recv(3200)
                            if not data:
                                print("Клиент аудио отключился")
                                break
                    
                            self.audio_queue.put(data)
                    
                        except Exception as e:
                            print(f"Ошибка приема аудио: {e}")
                            break

                    self._close_audio_connection()
                    print("Ожидание нового подключения для аудио...")

                except socket.timeout:
                    continue
                except Exception as e:
                    print(f"Ошибка в обработчике аудио: {e}")

        except Exception as e:
            print(f"Критическая ошибка в аудио-сервере: {e}")
        finally:
            if self.audio_socket:
                self.audio_socket.close()

    def handle_result_connection(self):
        """Обработчик соединения для результатов"""
        print("Ожидание подключения для результатов...")

        self.result_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.result_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

        try:
            self.result_socket.bind(('0.0.0.0', self.result_port))
            self.result_socket.listen(1)
            self.result_socket.settimeout(1.0)

            while self.running:
                try:
                    client_socket, client_address = self.result_socket.accept()
                    client_socket.settimeout(0.5)

                    print(f"Подключен клиент результатов: {client_address}")

                    if self.result_client:
                        self._close_result_connection()

                    self.result_client = client_socket

                    while self.running and self.result_client:
                        try:
                            data = self.result_client.recv(1)
                            if not data:
                                print("Клиент результатов отключился")
                                break
                        except socket.timeout:
                            continue
                        except:
                            break

                    self._close_result_connection()
                    print("Ожидание нового подключения для результатов...")

                except socket.timeout:
                    continue
                except Exception as e:
                    print(f"Ошибка в обработчике результатов: {e}")

        except Exception as e:
            print(f"Критическая ошибка в сервере результатов: {e}")
        finally:
            if self.result_socket:
                self.result_socket.close()

    def start(self):
        """Запуск сервера"""
        print("Запуск сервера для распознавания речи...")
        print(f"Порт для аудио: {self.audio_port}")
        print(f"Порт для результатов: {self.result_port}")

        self.running = True

        processing_thread = threading.Thread(target=self.audio_processing_worker)
        processing_thread.daemon = True
        processing_thread.start()

        audio_thread = threading.Thread(target=self.handle_audio_connection)
        audio_thread.daemon = True
        audio_thread.start()

        result_thread = threading.Thread(target=self.handle_result_connection)
        result_thread.daemon = True
        result_thread.start()

        print("Сервер запущен. Ожидание подключения Desktop приложения...")

        try:
            while self.running:
                command = input("Введите 'stop' для остановки сервера: ")
                if command.lower() == 'stop':
                    break
        except KeyboardInterrupt:
            print("\nОстановка сервера...")
        finally:
            self.stop()

    def stop(self):
        """Остановка сервера"""
        print("Остановка сервера...")
        self.running = False
        self.processing_active = False

        self._close_audio_connection()
        self._close_result_connection()

        while not self.audio_queue.empty():
            try:
                self.audio_queue.get_nowait()
                self.audio_queue.task_done()
            except:
                pass

if __name__ == "__main__":
    MODEL_PATH = "D:/AIs/vosk-model-small-ru-0.22"
    AUDIO_PORT = 9999
    RESULT_PORT = 9998

    server = SingleClientAudioServer(
        model_path=MODEL_PATH,
        audio_port=AUDIO_PORT,
        result_port=RESULT_PORT
    )
    server.start()