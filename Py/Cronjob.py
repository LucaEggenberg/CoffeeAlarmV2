import RPi.GPIO as GPIO
import time
from datetime import datetime, timedelta
import os
import logging

logging.basicConfig(filename='/coffee/log.txt', encoding='utf-8', level=logging.DEBUG, format='%(asctime)s %(message)s')

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)

def processFile(path: str, duration: int):
        f = open(path, 'r')
        dateObj = toDate(f.read().strip('\n'))
        f.close()
        if dateObj < datetime.now():
                logging.info('coffee time')
                os.remove(path)
                if getLastInit() + timedelta(minutes=5) < datetime.now():
                        initMachine()
                        time.sleep(35)
                makeCoffee(duration)

def makeCoffee(duration: int):
        GPIO.setup(17, GPIO.OUT, initial=GPIO.HIGH)
        time.sleep(duration)
        GPIO.setup(17, GPIO.OUT, initial=GPIO.LOW)

def initMachine():
        f = open('/coffee/init.txt','w')
        f.write(toDateString(datetime.now()))
        f.close()
        GPIO.setup(17, GPIO.OUT, initial=GPIO.LOW)
        time.sleep(1)
        GPIO.setup(17, GPIO.OUT, initial=GPIO.HIGH)
        time.sleep(0.5)
        GPIO.setup(17, GPIO.OUT, initial=GPIO.LOW)

def getLastInit() -> datetime:
        f = open('/coffee/init.txt')
        dateObj = toDate(f.read().strip('\n'))
        f.close()
        return dateObj

def toDate(dateStr: str) -> datetime:
        return datetime.strptime(dateStr, '%Y-%m-%d %H:%M:%S')

def toDateString(dateObj: datetime) -> str:
        return datetime.strftime(dateObj, '%Y-%m-%d %H:%M:%S')


if os.path.exists('/coffee/coffee.txt'):
        processFile('/coffee/coffee.txt', 38)

if os.path.exists('/coffee/espresso.txt'):
        processFile('/coffee/espresso.txt', 20)