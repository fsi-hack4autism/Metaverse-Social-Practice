import os
from openai import AzureOpenAI
from persona_system_prompts import PERSONAS


CLIENT = AzureOpenAI(
  azure_endpoint = "https://aoai-uc6-swed.openai.azure.com/", 
  api_key=os.getenv("AZURE_OPENAI_KEY"),  
  api_version="2024-02-15-preview"
)


class ResponseGenerator:
    def __init__(self, persona="friendly") -> None:
        self.persona = persona
        self.messages = [{
            "role" : "system",
            "content" : PERSONAS[persona]
        },
        {
            "role" : "user",
            "content" : ""
        }]
    
    def append_user_message(self, user_message) -> None:
        self.messages.append({
            "role" : "user",
            "content" : user_message
        })
    
    def append_assistant_message(self, assistant_message) -> None:
        self.messages.append({
            "role" : "assistant",
            "content" : assistant_message
        })
    
    def get_response(self) -> str:
        completion = CLIENT.chat.completions.create(
            model="gpt-35-turbo-16k",
            messages = self.messages,
            temperature=0.5,
            max_tokens=800,
            top_p=0.95,
            frequency_penalty=0,
            presence_penalty=0,
            stop=None
        )
        assistant_message = completion.choices[0].message.content
        self.append_assistant_message(assistant_message)
        return assistant_message


class UserSpeechToText:
    def __init__(self) -> None:
        self.deployment_id = "whisper-sweden"

    def get_user_text(self, audio_file_path) -> str:
        result = CLIENT.audio.transcriptions.create(
            file=open(audio_file_path, "rb"),            
            model=self.deployment_id
        )
        print(result)
        return result