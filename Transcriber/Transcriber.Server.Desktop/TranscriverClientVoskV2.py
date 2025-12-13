import socket
import wave
import threading
import time
import json

class SimpleTestClient:
    @staticmethod
    def send_test_audio(audio_file_path, host='localhost', audio_port=9999, result_port=9998):
        # Подключаемся к серверу результатов
        result_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        result_socket.connect((host, result_port))
        result_socket.settimeout(0.5)

        # Буфер для накопления данных
        buffer = ""

        def receive_results():
            nonlocal buffer
            while True:
                try:
                    data = result_socket.recv(4096)
                    if not data:
                        break
                    
                    # Добавляем данные в буфер
                    buffer += data.decode('utf-8')
                    
                    # Обрабатываем полные JSON строки
                    while '\n' in buffer:
                        line, buffer = buffer.split('\n', 1)
                        if line.strip():
                            try:
                                result = json.loads(line)
                                text = result.get('text', '')
                                result_type = result.get('type', '')
                                
                                if result_type == 'partial':
                                    print(f"\rПромежуточный: {text}\n", end='', flush=True)
                                elif result_type == 'final':
                                    print(f"\nФинальный: {text}\n")
                            except json.JSONDecodeError as e:
                                print(f"\nОшибка парсинга JSON: {e}")
                                print(f"Строка: {line}")
                                
                except socket.timeout:
                    continue
                except Exception as e:
                    print(f"\nОшибка: {e}")
                    break

        result_thread = threading.Thread(target=receive_results)
        result_thread.daemon = True
        result_thread.start()

        # Отправляем аудио
        audio_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        audio_socket.connect((host, audio_port))

        print(f"Отправка аудиофайла: {audio_file_path}\n")

        with wave.open(audio_file_path, 'rb') as wf:
            chunk_size = 3200
            while True:
                data = wf.readframes(chunk_size // 2)
                if not data:
                    break
                audio_socket.sendall(data)
                time.sleep(0.01)

        print("\nАудиофайл отправлен")
        time.sleep(10)  # Даем время на обработку
        
        audio_socket.close()
        result_socket.close()

SimpleTestClient.send_test_audio("C:/Users/dmitr/Downloads/test.wav")
