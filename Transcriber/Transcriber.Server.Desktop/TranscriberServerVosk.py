import sounddevice as sd
import numpy as np
import json
import time
import queue
import threading
import math
from vosk import Model, KaldiRecognizer

class AudioTranscriber:
    def __init__(self):
        self.sample_rate = 16000
        self.is_recording = False
        self.stream = None
        self.audio_queue = queue.Queue()
        self.processing_thread = None

        # Константа для включения/выключения фильтров
        self.APPLY_AUDIO_FILTERS = False  # True - фильтры включены, False - выключены

        # Параметры фильтров
        self.highpass_cutoff = 80  # Частота среза высокочастотного фильтра (Гц)
        self.noise_gate_threshold = 0.01  # Порог шумового гейта

        # Состояние фильтров
        self.prev_input = 0.0  # Предыдущее входное значение для high-pass фильтра
        self.prev_output = 0.0  # Предыдущее выходное значение для high-pass фильтра

        # Загружаем модель Vosk
        print("Загрузка модели Vosk...")
        self.model = Model("D:/AIs/vosk-model-small-ru-0.22")
        self.rec = KaldiRecognizer(self.model, self.sample_rate)

        # Выбираем устройство при запуске
        self.device_index = self.select_audio_device()

    def select_audio_device(self):
        """Показывает список устройств и позволяет выбрать одно"""
        print("\nДоступные аудиоустройства:")
        devices = sd.query_devices()

        input_devices = []
        for i, device in enumerate(devices):
            if device['max_input_channels'] > 0:
                input_devices.append((i, device))
                print(f"{len(input_devices)}. {device['name']} (входы: {device['max_input_channels']})")

        if not input_devices:
            print("Не найдено устройств ввода!")
            return None

        print(f"\nВсего устройств: {len(input_devices)}")
        print("Введите номер устройства (или Enter для выбора по умолчанию): ")

        try:
            choice = input().strip()
            if choice == "":
                print("Используется устройство по умолчанию")
                return None

            choice_idx = int(choice) - 1
            if 0 <= choice_idx < len(input_devices):
                selected_index, selected_device = input_devices[choice_idx]
                print(f"Выбрано устройство: {selected_device['name']}")
                return selected_index
            else:
                print("Неверный номер, используется устройство по умолчанию")
                return None
        except ValueError:
            print("Неверный ввод, используется устройство по умолчанию")
            return None

    def apply_highpass_filter(self, audio_data):
        """Применяет высокочастотный фильтр первого порядка (RC-фильтр)"""
        # Вычисляем коэффициент фильтра на основе частоты среза
        dt = 1.0 / self.sample_rate
        rc = 1.0 / (2 * math.pi * self.highpass_cutoff)
        alpha = rc / (rc + dt)

        filtered_data = np.zeros_like(audio_data)

        for i in range(len(audio_data)):
            # Формула high-pass RC-фильтра: y[i] = α * (y[i-1] + x[i] - x[i-1])
            self.prev_output = alpha * (self.prev_output + audio_data[i] - self.prev_input)
            filtered_data[i] = self.prev_output
            self.prev_input = audio_data[i]

        return filtered_data

    def apply_noise_gate(self, audio_data):
        """Применяет шумовой гейт для подавления фонового шума"""
        # Вычисляем RMS (среднеквадратичное значение) для определения уровня громкости
        rms = np.sqrt(np.mean(audio_data**2))

        # Если уровень сигнала ниже порога - подавляем шум
        if rms < self.noise_gate_threshold:
            # Подавляем шум, уменьшая амплитуду
            return audio_data * 0.1
        else:
            return audio_data

    def apply_audio_filters(self, audio_data):
        """Применяет все включенные аудиофильтры"""
        if not self.APPLY_AUDIO_FILTERS:
            return audio_data

        filtered_data = audio_data.copy()

        # Применяем высокочастотный фильтр
        filtered_data = self.apply_highpass_filter(filtered_data)

        # Применяем шумовой гейт
        filtered_data = self.apply_noise_gate(filtered_data)

        return filtered_data

    def audio_processing_worker(self):
        """Рабочий поток для обработки аудиоданных из очереди"""
        print("Запущен поток обработки аудио")
        if self.APPLY_AUDIO_FILTERS:
            print("Аудиофильтры ВКЛЮЧЕНЫ")
            print(f"  High-pass фильтр: {self.highpass_cutoff} Гц")
            print(f"  Шумовой гейт: порог {self.noise_gate_threshold}")
        else:
            print("Аудиофильтры ВЫКЛЮЧЕНЫ")

        while self.is_recording or not self.audio_queue.empty():
            try:
                # Получаем аудиоданные из очереди с таймаутом
                audio_bytes = self.audio_queue.get(timeout=1.0)

                # Обрабатываем через Vosk
                if self.rec.AcceptWaveform(audio_bytes):
                    result = json.loads(self.rec.Result())
                    print("Распознано:", result.get("text", ""))
                else:
                    partial = json.loads(self.rec.PartialResult())
                    print("Промежуточно:", partial.get("partial", ""))

                self.audio_queue.task_done()

            except queue.Empty:
                # Таймаут - проверяем, нужно ли продолжать работу
                continue
            except Exception as e:
                print(f"Ошибка при обработке аудио: {e}")
                continue

        print("Поток обработки аудио завершен")

    def audio_callback(self, indata, frames, time, status):
        """Callback-функция для захвата аудио"""
        if status:
            print(f"Audio status: {status}")

        # Берем оба канала и усредняем до моно
        if indata.shape[1] > 1:
            audio_data = np.mean(indata, axis=1)
        else:
            audio_data = indata.flatten()

        # Применяем фильтры, если они включены
        if self.APPLY_AUDIO_FILTERS:
            audio_data = self.apply_audio_filters(audio_data)

        # Преобразуем в байты для Vosk
        audio_bytes = (audio_data * 32767).astype(np.int16).tobytes()

        # Добавляем в очередь, если запись активна
        if self.is_recording:
            try:
                self.audio_queue.put(audio_bytes, timeout=0.1)
            except queue.Full:
                print("Очередь аудио переполнена, данные потеряны")

    def start_capture(self):
        """Запуск захвата аудио"""
        self.is_recording = True

        # Сбрасываем состояние фильтров
        self.prev_input = 0.0
        self.prev_output = 0.0

        try:
            # Получаем информацию о выбранном устройстве
            if self.device_index is not None:
                device_info = sd.query_devices(self.device_index)
                device_name = device_info['name']
                channels = min(2, device_info['max_input_channels'])
            else:
                device_info = sd.query_devices(sd.default.device[0])
                device_name = device_info['name']
                channels = 1

            print(f"Запуск захвата с устройства: {device_name}")

            self.stream = sd.InputStream(
                callback=self.audio_callback,
                channels=channels,
                samplerate=self.sample_rate,
                blocksize=3200,
                device=self.device_index,
                latency='low'
            )
            self.stream.start()

            # Запускаем поток обработки аудио
            self.processing_thread = threading.Thread(target=self.audio_processing_worker)
            self.processing_thread.daemon = True
            self.processing_thread.start()

            print(f"Захват аудио запущен!")
        except Exception as e:
            print(f"Ошибка при запуске захвата аудио: {e}")
            print("Попытка использовать устройство по умолчанию...")
            # Пробуем устройство по умолчанию
            try:
                self.stream = sd.InputStream(
                    callback=self.audio_callback,
                    channels=1,
                    samplerate=self.sample_rate,
                    blocksize=int(self.sample_rate * 0.1),
                    latency='low'
                )
                self.stream.start()

                # Запускаем поток обработки аудио
                self.processing_thread = threading.Thread(target=self.audio_processing_worker)
                self.processing_thread.daemon = True
                self.processing_thread.start()

            except Exception as e2:
                print(f"Критическая ошибка: {e2}")
                self.is_recording = False

    def stop_capture(self):
        """Остановка захвата аудио"""
        self.is_recording = False
        if self.stream:
            self.stream.stop()
            self.stream.close()

        # Ждем завершения потока обработки
        if self.processing_thread and self.processing_thread.is_alive():
            self.processing_thread.join(timeout=2.0)
            if self.processing_thread.is_alive():
                print("Поток обработки не завершился корректно")

    def run(self):
        """Основной цикл приложения"""
        self.start_capture()

        if not self.is_recording:
            print("Не удалось запустить захват аудио")
            return

        try:
            while self.is_recording:
                time.sleep(0.1)  # Минимальная задержка
        except KeyboardInterrupt:
            print("\nОстановка приложения...")
        finally:
            self.stop_capture()

if __name__ == "__main__":
    transcriber = AudioTranscriber()
    transcriber.run()