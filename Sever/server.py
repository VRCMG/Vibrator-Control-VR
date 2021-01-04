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

def getEntryfromDB(accesscode):
    global toys

    query = { "$or": [{"_id": accesscode}, {"short": accesscode}] }

    enties = toys.find(query)

    for entry in enties:
        return entry
    else:
        return None


def deleteAccesscodeFromDB(accesscode):
    global toys
    query = { "$or": [{"_id": accesscode}, {"short": accesscode}] }
    toys.delete_one(query)

def insertToy(uid, utoken, toy, short):
    global toys
    entry = { "uid": uid, "utoken": utoken, "toy":toy, "_id":uid, "short": short }
    toys.insert_one(entry)

@app.route('/')
def default():
    global secret
    uid = uuid.uuid4().hex
    short = uid[:6]
    utoken = sha256(f"{uid}:{secret}".encode('utf-8')).hexdigest()

    url = f"https://api.lovense.com/api/lan/getQrCode?token={token}&uid={uid}&uname=user&utoken={utoken}"
    response = requests.request("POST", url)
    url = response.json()["message"]
    return render_template("index.html", url=url, accesscode=uid, short=short)

@app.route('/remove/<accesscode>')
def removeAccesscode(accesscode): 
    global sessions

    deleteAccesscodeFromDB(accesscode)

    if accesscode in sessions:
        del sessions[accesscode]

    return 'Deleted'

@app.route('/sendCommand', methods = ['POST'])
def sendCommand():
    global sessions


    accesscode = request.json["accesscode"]
    action = request.json["action"]
    value = request.json["value"]

    entry = getEntryfromDB(accesscode)
    if(entry is None):
        return 'Invalid code'

    accesscode = entry['uid']
    toy = entry['toy'][0]

    url = f"https://api.lovense.com/api/lan/command?token={token}&uid={accesscode}&t={toy}&v={value}&command={action}"

    response = requests.request("POST", url)

    if(response.json()["message"] is not None):
        return 'Error'

    return 'Success'

@app.route('/callback', methods = ['POST'])
def callback():
    global toys, secret
    content = request.get_json()
    toy = []
    for (k, _) in content['toys'].items():
        toy.append(k)
    uid = content['uid']
    utoken = content['utoken']

    utokenCheck = sha256(f"{uid}:{secret}".encode('utf-8')).hexdigest()
    
    entry = getEntryfromDB(uid)
    if(entry is not None):
        #refresh check against old utoken
        utokenCheck = entry['utoken']


    if(utoken != utokenCheck):
        print("[Callback] UID: "+uid)
        print("[Callback] utoken: " + utoken)
        print("[Callback] Expected: " + utokenCheck)
        print("[Callback] Invalid token")
        return "Invalid"

    if(entry is not None):
        print("[Callback] Updateing toys")
        deleteAccesscodeFromDB(uid)
    else:
        print("[Callback] Registering toys")
    insertToy(uid, utoken, toy, uid[:6])
    return ''


app.run(host='0.0.0.0', port= 8090, debug=True)