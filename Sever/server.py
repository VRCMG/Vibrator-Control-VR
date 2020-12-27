from flask import Flask, request, render_template
from hashlib import sha256
import pymongo, requests, os, sys, uuid, random, string


app = Flask(__name__)
mongo = pymongo.MongoClient("mongodb://mongo:27017/")
toys = mongo["lovense"]["toys"]

token = os.environ.get('token')
secret = os.environ.get('secret')

sessions = {}

def get_random_string(length = 20):
    letters = string.ascii_letters
    result_str = ''.join(random.choice(letters) for i in range(length))
    return result_str

if(secret is None):
    secret = get_random_string()

if token is None:
    print("Not token provided")
    sys.exit(-1)

def getToyForID(accesscode):
    global toys

    query = { "_id": accesscode }

    toy = toys.find(query)

    return toy[0]["toy"]

@app.route('/')
def hello():
    global secret
    uid = uuid.uuid4().hex
    utoken = sha256(f"{uid}:{secret}".encode('utf-8')).hexdigest()

    url = f"https://api.lovense.com/api/lan/getQrCode?token={token}&uid={uid}&uname=test&utoken={utoken}"
    response = requests.request("POST", url)
    url = response.json()["message"]
    print(url)
    return render_template("index.html", url=url, accesscode=uid)

def startSession(session):
    url = f"https://api.lovense.com/developer/v2/play/{session}"
    _ = requests.request("GET", url)


def createSession(accesscode):
    global token
    
    toy = getToyForID(accesscode)

    url = f"https://api.lovense.com/developer/v2/createSession?token={token}&customerid=0&expires=0&toyId={toy}"

    response = requests.request("POST", url)
    sid = response.json()["data"]["sid"]
    startSession(sid)
    print(sid)
    return sid

def endSession():
    pass


@app.route('/sendCommand', methods = ['POST'])
def sendCommand():
    global sessions

    accesscode = request.json["accesscode"]
    action = request.json["action"]
    value = request.json["value"]

    if accesscode not in sessions:
        sessions[accesscode] = createSession(accesscode)

    url = f"https://api.lovense.com/developer/v2/sendCommand/{sessions[accesscode]}?type={action}&value={value}"

    response = requests.request("POST", url)

    if(response.json()["message"] is not None):
        return 'Error'

    return 'Success'

@app.route('/callback', methods = ['POST'])
def callback():
    global toys, secret
    content = request.get_json()
    print(request.json)
    toy = ""
    for (k, _) in content['toys'].items():
        toy = k
    uid = content['uid']
    utoken = content['utoken']

    utokenCheck = sha256(f"{uid}:{secret}".encode('utf-8')).hexdigest()

    print("UID: "+uid)
    print("utoken: " + utoken)
    print("Expected: " + utokenCheck)
    if(utoken != utokenCheck):
        print("Invalid token")
        return "Invalid"

    print("Registering")
    print("Toy: " + toy)
    entry = { "uid": uid, "utoken": utoken, "toy":toy, "_id":uid }
    toys.insert_one(entry)
    print("Registered")
    return ''


app.run(host='0.0.0.0', port= 8090, debug=True)