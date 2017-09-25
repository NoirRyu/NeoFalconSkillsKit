using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Microsoft.Azure.Devices.Client;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;


using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.System;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BackgroundTemp
{
    public sealed class StartupTask : IBackgroundTask
    {

        private DeviceClient deviceClient;
        
        private const int LED_PIN = 21;  //62
        private const int PINOUT_PIN = 62;
        private GpioPin OnBoard, PinOut;
        private GpioPinValue pinValue;

        private LED_Control LEDControl;
        private I2cDevice ledControl;

        BackgroundTaskDeferral deferral; 

        public void Run(IBackgroundTaskInstance taskInstance)
        {          
            
            deviceClient = DeviceClient.CreateFromConnectionString("========Your Connection String=========", TransportType.Http1);
            

            InitGPIO();
            InitializeI2CDevice();

            deferral = taskInstance.GetDeferral();
            
            if (deviceClient != null)
            {
                ReceiveDataFromAzure();
            }
            else
            {
                deferral.Complete();
            }
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            OnBoard = gpio.OpenPin(LED_PIN);
            PinOut = gpio.OpenPin(PINOUT_PIN); 

            OnBoard.Write(GpioPinValue.High);
            PinOut.Write(GpioPinValue.High);

            OnBoard.SetDriveMode(GpioPinDriveMode.Output);
            PinOut.SetDriveMode(GpioPinDriveMode.Output);
        }

        private async void InitializeI2CDevice()
        {
            try
            {
                string advanced_query_syntax = I2cDevice.GetDeviceSelector(LED_Control.I2C_CONTROLLER_NAME);
                DeviceInformationCollection device_information_collection =
                    await DeviceInformation.FindAllAsync(advanced_query_syntax);
                string deviceId = device_information_collection[0].Id;

                I2cConnectionSettings ledControl_connection =
                    new I2cConnectionSettings(LED_Control.LEDCONTROL_ADDR);
                ledControl_connection.BusSpeed = I2cBusSpeed.FastMode;
                ledControl_connection.SharingMode = I2cSharingMode.Shared;

                ledControl = await I2cDevice.FromIdAsync(deviceId, ledControl_connection); 

                byte[] WriteBuf_PowerControl = new byte[] { LED_Control.LEDCONTROL_CONTROL, 0x03 };
                ledControl.Write(WriteBuf_PowerControl); 

                LEDControl = new LED_Control(ref ledControl);
                
            }
            catch (Exception e)
            {                
                return;
            }
        }
        
       
        public async void ReceiveDataFromAzure()
        {
            Message receivedMessage;
            string messageData;            

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();
                
                if(receivedMessage != null)
                {
                    try
                    {                       
                        messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                        if (messageData == "accessaryLightOn")
                        {
                            LEDControl.setLight();                             
                            await deviceClient.CompleteAsync(receivedMessage);
                        }
                        else if (messageData == "accessaryLightOff")
                        {
                             LEDControl.setOFF(); 
                             await deviceClient.CompleteAsync(receivedMessage);
                        }
                        else if (messageData == "PinOutLightOn")
                        {
                            PinOut.Write(GpioPinValue.Low);
                            await deviceClient.CompleteAsync(receivedMessage);
                        }
                        else if (messageData == "PinOutLightOff")
                        {
                            PinOut.Write(GpioPinValue.High);
                            await deviceClient.CompleteAsync(receivedMessage);
                        }
                        else if (messageData == "OnBoardLightOn")
                        {
                            OnBoard.Write(GpioPinValue.High);                           
                            await deviceClient.CompleteAsync(receivedMessage);
                        }
                        else if (messageData == "OnBoardLightOff")
                        {
                            OnBoard.Write(GpioPinValue.Low);
                            await deviceClient.CompleteAsync(receivedMessage);
                        }
                    }
                    catch
                    {
                        return; 
                    }
                }
            }
        }
    }
}
