//#include "BluetoothSerial.h"
#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>
#include "BluetoothSerial.h"

//#define servoPWM 12
#define turn0 15
#define turn1 16

/*
double      ledcSetup(uint8_t channel, double freq, uint8_t resolution_bits);
void        ledcWrite(uint8_t channel, uint32_t duty);
double      ledcWriteTone(uint8_t channel, double freq);
double      ledcWriteNote(uint8_t channel, note_t note, uint8_t octave);
uint32_t    ledcRead(uint8_t channel);
double      ledcReadFreq(uint8_t channel);
void        ledcAttachPin(uint8_t pin, uint8_t channel);
void        ledcDetachPin(uint8_t pin);
*/

//servo settings
const int servoPWM = 14;
const int pwmFreq = 400;
const int pwmRes = 8;
const int pwmChan = 0;



//BluetoothSerial espBT;
#define SERVICE_UUID           "f4f80098-5ba3-11ec-bf63-0242ac130002"
#define CHARACTERISTIC_UUID_RX "f4f8061a-5ba3-11ec-bf63-0242ac130002"
#define CHARACTERISTIC_UUID_TX "f4f80778-5ba3-11ec-bf63-0242ac130002"

//int recv;
BLECharacteristic *pCharacteristic;
BLECharacteristic *pCharacteristicRX;
bool deviceConnected = false;


class RecvCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristicRX) {
      std::string recv = pCharacteristicRX->getValue();

      if (recv.length() > 0) {
        
        //print what recv from mobile app
        Serial.print("Received: ");
        for (int i = 0; i < recv.length(); i++) {
          Serial.print(recv[i]);
        }
        Serial.println();

        //check for what inputs we receive
        if (recv.find("A") == true) {
          Serial.println("ROTATING MOTOR (PRETEND)");
        }
      
      }
    }
};


class ServerConnectionCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      deviceConnected = true;
    };

    void onDisconnect(BLEServer* pServer) {
      deviceConnected = false;
    }
};



void setup() {

  //setup pins
  ledcAttachPin(servoPWM, pwmChan);
  ledcSetup(pwmChan, pwmFreq, pwmRes);
  

  Serial.begin(9600);  
  //espBT.begin("ESP32");
  BLEDevice::init("ESP32 hh");
  BLEServer *pServer = BLEDevice::createServer();
  pServer->setCallbacks(new ServerConnectionCallbacks());

  BLEService *pService = pServer->createService(SERVICE_UUID);
  pCharacteristic = pService->createCharacteristic(
                                         CHARACTERISTIC_UUID_TX,
                                         BLECharacteristic::PROPERTY_NOTIFY |
                                         //BLECharacteristic::PROPERTY_WRITE |
                                         BLECharacteristic::PROPERTY_READ
                                       );
  pCharacteristic->addDescriptor(new BLE2902());

  pCharacteristicRX = pService->createCharacteristic(
                                         CHARACTERISTIC_UUID_RX,
                                         BLECharacteristic::PROPERTY_WRITE
                                       );
  pCharacteristicRX->setCallbacks(new RecvCallbacks());


  pService->start();

  pServer->getAdvertising()->start();
  Serial.println("waiting for connection ...");

}

int i = 0;
float txValue = 0;
void loop() {

  if (deviceConnected) {

    // Let's convert the value to a char array:
    char txString[8]; 
    dtostrf(txValue, 1, 2, txString); // float_val, min_width, digits_after_decimal, char_buffer
    
//    pCharacteristic->setValue(&txValue, 1); // To send the integer value
//    pCharacteristic->setValue("Hello!"); // Sending a test message
    pCharacteristic->setValue(txString);
    
    pCharacteristic->notify();
    Serial.print("Sent: ");
    Serial.println(txString);

  } else {
    Serial.println("Device disconnected. . . ");
    delay(4000);
  }

  delay(1000);
  txValue++;
  /*if(turn0 == HIGH){
    ledcWrite(pwmChan, 50);
  } else if(turn1 == HIGH){
    ledcWrite(pwmChan, 150);
  }*/

  /*for(int dutyCycle = 0; dutyCycle <= 255; dutyCycle++){   
    // changing the LED brightness with PWM
    ledcWrite(pwmChan, dutyCycle);
    delay(15);
  }

  // decrease the LED brightness
  for(int dutyCycle = 255; dutyCycle >= 0; dutyCycle--){
    // changing the LED brightness with PWM
    ledcWrite(pwmChan, dutyCycle);   
    delay(15);
  }*/
  


}
