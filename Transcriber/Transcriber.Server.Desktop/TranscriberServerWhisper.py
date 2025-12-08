from transformers import WhisperProcessor, WhisperForConditionalGeneration
import torchaudio
import torch
import sounddevice as sd
import numpy as np
import threading
from collections import deque
import time
import os
import soundfile as sf
from datetime import datetime

model_path = "D:/AIs/whisper-tiny"
processor = WhisperProcessor.from_pretrained(model_path)
model = WhisperForConditionalGeneration.from_pretrained(model_path)

class AudioTranscriber:
    def __init__(self):
        self.sample_rate = 16000
        self.chunk_duration = 5  # seconds
        self.buffer = deque(maxlen=self.sample_rate * self.chunk_duration * 2)
        self.is_recording = False
        self.stream = None

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

    def start_capture(self):
        """Запуск захвата аудио"""
        self.is_recording = True

        def audio_callback(indata, frames, time, status):
            if status:
                print(f"Audio status: {status}")
            # Берем оба канала и усредняем до моно
            if indata.shape[1] > 1:
                audio_data = np.mean(indata, axis=1)
            else:
                audio_data = indata.flatten()
            self.buffer.extend(audio_data)

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
                callback=audio_callback,
                channels=channels,
                samplerate=self.sample_rate,
                blocksize=int(self.sample_rate * 0.1),  # 100ms блоки
                device=self.device_index,
                latency='low'
            )
            self.stream.start()
            print(f"Захват аудио запущен!")
        except Exception as e:
            print(f"Ошибка при запуске захвата аудио: {e}")
            print("Попытка использовать устройство по умолчанию...")
            # Пробуем устройство по умолчанию
            try:
                self.stream = sd.InputStream(
                    callback=audio_callback,
                    channels=1,
                    samplerate=self.sample_rate,
                    blocksize=int(self.sample_rate * 0.1),
                    latency='low'
                )
                self.stream.start()
            except Exception as e2:
                print(f"Критическая ошибка: {e2}")
                self.is_recording = False

    def stop_capture(self):
        """Остановка захвата аудио"""
        self.is_recording = False
        if self.stream:
            self.stream.stop()
            self.stream.close()

    def transcribe_audio(self):
        """Транскрипция аудио из буфера"""
        if len(self.buffer) < self.sample_rate * self.chunk_duration:
            print("Буфер не заполнен")
            return None

        # Берем последние N секунд из буфера
        audio_data = np.array(list(self.buffer)[-self.sample_rate * self.chunk_duration:])

        # Нормализация
        max_val = np.max(np.abs(audio_data))
        if max_val > 0:
            audio_data = audio_data.astype(np.float32) / max_val
        else:
            audio_data = audio_data.astype(np.float32)

        input_features = processor(
            audio_data,
            sampling_rate=self.sample_rate,
            return_tensors="pt"
        ).input_features

        predicted_ids = model.generate(
            input_features,
            language="russian",
            task="transcribe"
        )

        transcription = processor.batch_decode(
            predicted_ids,
            skip_special_tokens=True
        )

        return transcription[0]

    def run(self):
        """Основной цикл приложения"""
        self.start_capture()

        if not self.is_recording:
            print("Не удалось запустить захват аудио")
            return

        try:
            while self.is_recording:
                transcription = self.transcribe_audio()
                if transcription and len(transcription.strip()) > 0:
                    print(f"Транскрипция: {transcription}")
                time.sleep(2)  # Пауза между обработками
        except KeyboardInterrupt:
            print("\nОстановка приложения...")
        finally:
            self.stop_capture()

if __name__ == "__main__":
    transcriber = AudioTranscriber()
    transcriber.run()