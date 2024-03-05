from pydantic import BaseModel
from datetime import datetime
import RPi.GPIO as GPIO
import os

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)

app = FastAPI()

class SetGPIO(BaseModel):
        on: bool

coffeePath = '/coffee/coffee.txt'
espressoPath = '/coffee/espresso.txt'

@app.get("/read/{gpio}")
def read_root(gpio: int):
    GPIO.setup(gpio, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
    return {"gpio": gpio, "on": GPIO.input(gpio)}

@app.patch("/set/{gpio}")
def read_item(gpio: int, value: SetGPIO):
    if value.on:
        GPIO.setup(gpio, GPIO.OUT, initial=GPIO.HIGH)
    else:
        GPIO.setup(gpio, GPIO.OUT, initial=GPIO.LOW)
    return {"gpio": gpio, "on": value.on}


@app.put("/timer/coffee")
def set_coffee_timer(time: datetime):
        makeFile(coffeePath, time)

@app.put("/timer/espresso")
def set_espresso_timer(time: datetime):
        makeFile(espressoPath, time)

@app.delete("/timer")
def delete_timers():
        deleteFiles()

@app.get("/timer")
def get_active_timer()
        if os.path.exists(coffeePath):
                return {"coffee": "coffee", "time": getTime(coffeePath)}
        if os.path.exists(espressoPath):
                return {"coffee": "espresso", "time": getTime(espressoPath)}
        return {"coffee": None, "time": None}

def makeFile(path: string, time: datetime):
        deleteFiles()
        f = open(path, 'x')
        f.write(time.strftime('%Y-%m-%d %H:%M:%S'))
        f.close()

def deleteFiles():
        if os.path.exists(coffeePath):
                os.remove(coffeePath)
        if os.path.exists(espressoPath):
                os.remove(espressoPath)

def getTime(path: string) -> datetime:
        return datetime.strptime(open(path, 'r').read(), '%Y-%m-%d %H:%M:%S')