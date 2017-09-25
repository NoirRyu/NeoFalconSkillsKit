using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.I2c; 

namespace BackgroundTemp
{
    class LED_Control
    {
        public const byte LEDCONTROL_ADDR = 0x33;
        public const byte LEDCONTROL_CONTROL = 0x00;
        private const byte LEDCONTROL_LIGHT = 0x47;
        private const byte LEDCONTROL_DARK = 0x41;
        private const byte LEDCONTROL_OFF = 0x00;
        private const byte LEDCONTROL_LED_BLINK = 0x40;

        public const string I2C_CONTROLLER_NAME = "I2C1";   //I2C NAME for NeoFalcon

        private I2cDevice I2CLight;
        public LED_Control(ref I2cDevice I2CDevice)
        {
            this.I2CLight = I2CDevice;
        }

        public void setLight()
        {           
            setLed_Control(0x47);
        }
        public void setDark()
        {            
            setLed_Control(0x41);
        }
        public void setOFF()
        {            
            setLed_Control(0x00);
        }

        private ushort I2CRead16(byte addr)
        {
            byte[] address = new byte[] { (byte)(addr) };
            byte[] data = new byte[1];            

            I2CLight.WriteRead(address, data);

            return (ushort)data[0];
        }

        private void setLed_Control(byte writingByte)
        {
            uint[] Data = new uint[2];

            Data[0] = I2CRead16(0x01);
            byte bByte = (byte)Data[0];

            byte[] WriteBuf_PowerControl = new byte[] { LEDCONTROL_CONTROL, writingByte };

            try
            {               
                I2CLight.Write(WriteBuf_PowerControl);
            }
            catch (Exception)
            {
                return;
            }

        }




    }
}
