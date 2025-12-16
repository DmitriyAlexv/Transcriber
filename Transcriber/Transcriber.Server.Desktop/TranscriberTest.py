from transformers import WhisperProcessor, WhisperForConditionalGeneration
import torchaudio
import torch

file_path_test = "C:/Users/dmitr/Downloads/test.mp3"
model_path = "D:/AIs/whisper-tiny"
processor = WhisperProcessor.from_pretrained(model_path)
model = WhisperForConditionalGeneration.from_pretrained(model_path)

def transcribe_audio_file(file_path):
    waveform, sample_rate = torchaudio.load(file_path)

    if waveform.shape[0] > 1:
        waveform = torch.mean(waveform, dim=0, keepdim=True)

    if sample_rate != 16000:
        resampler = torchaudio.transforms.Resample(sample_rate, 16000)
        waveform = resampler(waveform)
        sample_rate = 16000

    audio_array = waveform.squeeze().numpy()

    input_features = processor(
        audio_array,
        sampling_rate=sample_rate,
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

result = transcribe_audio_file(file_path_test)
print(f"Результат: {result}")