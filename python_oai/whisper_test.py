import os
from openai import AzureOpenAI

client = AzureOpenAI(
    api_key=os.getenv("AZURE_OPENAI_API_KEY"),  
    api_version="2024-02-01",
    azure_endpoint = os.getenv("AZURE_OPENAI_ENDPOINT")
)

deployment_id = "whisper-sweden"
audio_test_file = os.path.join("sample_data", "restaurant-conversation.mp3")

result = client.audio.transcriptions.create(
    file=open(audio_test_file, "rb"),            
    model=deployment_id
)

print(result)