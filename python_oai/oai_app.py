from flask import Flask
from response_generator import ResponseGenerator, UserSpeechToText

app = Flask(__name__)
rg = ResponseGenerator()
ustt = UserSpeechToText()

@app.route("/get-response/<audio_file>")
def get_response(audio_file):
    user_message = ustt.get_user_text(audio_file)
    rg.append_user_message(user_message)
    response = rg.get_response()
    return {response}

@app.route("/setup-chat/<persona>")
def setup_chat(persona):
    global rg
    rg = ResponseGenerator(persona)
    return

@app.route("/clear-chat/")
def get_response():
    return {}