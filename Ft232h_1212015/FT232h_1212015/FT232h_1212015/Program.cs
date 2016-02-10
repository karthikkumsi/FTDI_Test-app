using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
//FTDI Imports
using FTD2XX_NET;


namespace FT232h_1212015
{
    public class FTDI
    {

       
        private enum HI_SPEED_DEVICE_TYPES
        {
            FT2232H_DEVICE_TYPE = 1,
            FT4232H_DEVICE_TYPE = 2
        };

        public const uint I2C_TRANSFER_OPTIONS_START_BIT = 0x01;
        public const uint I2C_TRANSFER_OPTIONS_BREAK_ON_NACK = 0x02;
        public const uint I2C_TRANSFER_OPTIONS_STOP_BIT = 0x04;
        private const uint I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BYTES = 0x10;
        private const uint I2C_TRANSFER_OPTIONS_FAST_TRANSFER_BITS = 0x20;


        private const uint FTC_SUCCESS = 0;
        private const uint FTC_DEVICE_IN_USE = 27;

        private const uint MAX_NUM_DEVICE_NAME_CHARS = 100;
        private const uint MAX_NUM_CHANNEL_CHARS = 5;

        private const uint MAX_NUM_DLL_VERSION_CHARS = 10;
        private const uint MAX_NUM_ERROR_MESSAGE_CHARS = 100;

        
        private const uint MAX_FREQ_M24C64_CLOCK_DIVISOR = 79;  // equivalent to 375KHz

        private const uint STANDARD_MODE = 1;
        private const uint FAST_MODE = 2;
        private const uint STRETCH_DATA_MODE = 4;

        private const uint WRITE_CONTROL_BUFFER_SIZE = 256;
        private const uint WRITE_DATA_BUFFER_SIZE = 65536;
        private const uint READ_DATA_BUFFER_SIZE = 65536;

        private const uint MAX_I2C_M24C64_CHIP_SIZE_IN_BYTES = 512;

        private const uint NUM_M24C64_PAGES = 1;
        private const uint NUM_M24C64_BYTES_PER_PAGE = 32;

        private const uint MAX_I2C_24LC16B_CHIP_SIZE_IN_BYTES = 256;

        private const uint NUM_24LC16B_PAGES = 8;
        private const uint NUM_24LC16B_BYTES_PER_PAGE = 256;

        private const uint NO_WRITE_TYPE = 0;
        private const uint BYTE_WRITE_TYPE = 1;
        private const uint PAGE_WRITE_TYPE = 2;

        private const uint BYTE_READ_TYPE = 1;
        private const uint PAGE_READ_TYPE = 2;


        //**************************************************************************
        //
        // TYPE DEFINITIONS
        //
        //**************************************************************************

        public struct FTC_INPUT_OUTPUT_PINS
        {
            public bool bPin1InputOutputState;
            public bool bPin1LowHighState;
            public bool bPin2InputOutputState;
            public bool bPin2LowHighState;
            public bool bPin3InputOutputState;
            public bool bPin3LowHighState;
            public bool bPin4InputOutputState;
            public bool bPin4LowHighState;
        }

        public struct FTH_INPUT_OUTPUT_PINS
        {
            public bool bPin1InputOutputState;
            public bool bPin1LowHighState;
            public bool bPin2InputOutputState;
            public bool bPin2LowHighState;
            public bool bPin3InputOutputState;
            public bool bPin3LowHighState;
            public bool bPin4InputOutputState;
            public bool bPin4LowHighState;
            public bool bPin5InputOutputState;
            public bool bPin5LowHighState;
            public bool bPin6InputOutputState;
            public bool bPin6LowHighState;
            public bool bPin7InputOutputState;
            public bool bPin7LowHighState;
            public bool bPin8InputOutputState;
            public bool bPin8LowHighState;
        }

        public struct FTC_LOW_HIGH_PINS
        {
            public bool bPin1LowHighState;
            public bool bPin2LowHighState;
            public bool bPin3LowHighState;
            public bool bPin4LowHighState;
        }

        public struct FTC_PAGE_WRITE_DATA
        {
            public uint NumPages;
            public uint NumBytesPerPage;
        }

        public struct FTH_LOW_HIGH_PINS
        {
            public bool bPin1LowHighState;
            public bool bPin2LowHighState;
            public bool bPin3LowHighState;
            public bool bPin4LowHighState;
            public bool bPin5LowHighState;
            public bool bPin6LowHighState;
            public bool bPin7LowHighState;
            public bool bPin8LowHighState;
        }

        public struct FTC_CLOSE_FINAL_STATE_PINS
        {
            public bool bTCKPinState;
            public bool bTCKPinActiveState;
            public bool bTDIPinState;
            public bool bTDIPinActiveState;
            public bool bTMSPinState;
            public bool bTMSPinActiveState;
        }



        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ChannelCfg
        {
            public UInt32 ClockRate;
            public byte LatencyTimer;
            public UInt32 Options;
        }

        // Built-in Windows API functions to allow us to dynamically load our own DLL.
        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_GetNumChannels", CallingConvention = CallingConvention.Cdecl)]
        static extern uint I2C_GetNumChannels(ref uint NumChannels);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_OpenChannel", CallingConvention = CallingConvention.Cdecl)]
        static extern uint I2C_OpenChannel(uint index, ref IntPtr handler);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_CloseChannel", CallingConvention = CallingConvention.Cdecl)]
        static extern uint I2C_CloseChannel(IntPtr handler);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_InitChannel", CallingConvention = CallingConvention.Cdecl)]
        static extern uint I2C_InitChannel(IntPtr handler, ref ChannelCfg config);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceRead", CallingConvention = CallingConvention.Cdecl)]
        static extern uint I2C_DeviceRead(IntPtr handler, UInt32 deviceAddress, UInt32 sizeToTransfer, byte[] buffer, ref UInt32 sizeTransfered, UInt32 options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "I2C_DeviceWrite", CallingConvention = CallingConvention.Cdecl)]
        static extern uint I2C_DeviceWrite(IntPtr handler, UInt32 deviceAddress, UInt32 sizeToTransfer, byte[] buffer, ref UInt32 sizeTransfered, UInt32 options);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_WriteGPIO", CallingConvention = CallingConvention.Cdecl)]
        static extern uint FT_WriteGPIO(IntPtr handler, byte dir, byte value);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "FT_ReadGPIO", CallingConvention = CallingConvention.Cdecl)]
        static extern uint FT_ReadGPIO(IntPtr handler, ref  byte value);

        [DllImportAttribute("libMPSSE.dll", EntryPoint = "Init_libMPSSE", CallingConvention = CallingConvention.Cdecl)]
        static extern void Init_libMPSSE();

        private uint channel = 0;
        IntPtr FTDIhandler;

        private byte dir = 0;
        private byte gpo = 0;

        private System.Object _lock = new System.Object();

        public static FTDI Instance = new FTDI();

        public bool isConnected()
        {
            if (FTDIhandler == IntPtr.Zero)
            {
                return false;
            }
            return true;
        }


        public int Init(uint channel)
        {
            uint ch = 0;
            uint status = 0;
            if (FTDIhandler != IntPtr.Zero)
            {

                status = I2C_GetNumChannels(ref ch);
                if (status == 0 && ch == 2)
                {
                    Console.WriteLine("FTDI is in use");
                    return -111;
                }
            }
            this.channel = channel;

            FTDIhandler = new IntPtr();
            Init_libMPSSE();
            I2C_GetNumChannels(ref ch);

            if (ch > channel)
            {
                status = I2C_OpenChannel(channel, ref FTDIhandler);
                if (status != 0)
                {
                    //   MessageBox.Show("FTDI error while open channel " + channel);
                    Console.WriteLine("FTDI error while open channel" + channel);
                    I2C_CloseChannel(FTDIhandler);
                    return -1;
                }
                ChannelCfg chcfg;
                chcfg.ClockRate = 40000;
                chcfg.LatencyTimer = 150;
                chcfg.Options = 3;
                status = I2C_InitChannel(FTDIhandler, ref chcfg);
                if (status != 0)
                {
                    Console.WriteLine("FTDI error while init channel" + ch);
                    
                    I2C_CloseChannel(FTDIhandler);
                    return -1;
                }
            }
            else
            {
                Console.WriteLine("ERROR while init FTDI only " + ch + " are available, can't set channel " + channel + "!");
                
                return -1;
            }
            return 0;
        }

        public uint I2Cread(UInt32 deviceAddress, UInt32 sizeToTransfer, byte[] buffer, ref UInt32 sizeTransfered, UInt32 options)
        {
            uint status = 0;
            if (FTDIhandler == IntPtr.Zero)
            {
                return 111;
            }

            lock (_lock)
            {
                status = I2C_DeviceRead(FTDIhandler, deviceAddress, sizeToTransfer, buffer, ref sizeTransfered, options);
            }
            return status;
        }

        public uint I2Cwrite(UInt32 deviceAddress, UInt32 sizeToTransfer, byte[] buffer, ref UInt32 sizeTransfered, UInt32 options)
        {
            uint status = 0;
            if (FTDIhandler == IntPtr.Zero)
            {
                return 111;
            }
            lock (_lock)
            {
                status = I2C_DeviceWrite(FTDIhandler, deviceAddress, sizeToTransfer, buffer, ref sizeTransfered, options);
            }
            return status;
        }

        //pin = pin number; dir = 0:=input; 1:=output
        public uint setGPIOdir(byte pin, byte dir)
        {
            uint status = 0;
            if (FTDIhandler == IntPtr.Zero)
            {
                return 111;
            }
            if (dir == 1)
            {
                this.dir |= (byte)(1 << pin);
            }
            else
            {
                this.dir &= ((byte)~(1 << pin));
            }
            this.gpo &= ((byte)~(1 << pin));

            lock (_lock)
            {
                status = FT_WriteGPIO(FTDIhandler, this.dir, this.gpo);
            }
            return status;
        }

        public uint setGPIO(byte pin)
        {
            uint status = 0;
            if (FTDIhandler == IntPtr.Zero)
            {
                return 111;
            }
            this.gpo = (byte)(this.gpo | (byte)(1 << pin));
            lock (_lock)
            {
                status = FT_WriteGPIO(FTDIhandler, this.dir, this.gpo);
            }
            return status;
        }

        public uint clearGPIO(byte pin)
        {
            uint status = 0;
            if (FTDIhandler == IntPtr.Zero)
            {
                return 111;
            }
            this.gpo &= ((byte)~(1 << pin));
            lock (_lock)
            {
                status = FT_WriteGPIO(FTDIhandler, this.dir, this.gpo);
            }
            return status;
        }

        public uint readGPIO(ref byte value)
        {
            uint status = 0;
            if (FTDIhandler == IntPtr.Zero)
            {
                return 111;
            }
            lock (_lock)
            {
                status = FT_ReadGPIO(FTDIhandler, ref  value);
            }
            return status;
        }


        public uint close()
        {
            if (FTDIhandler == IntPtr.Zero)
            {
                return 111;
            }
            uint status = I2C_CloseChannel(FTDIhandler);
            FTDIhandler = IntPtr.Zero;
            return status;
        }

        public int connectionState()
        {
            uint ch = 0;
            uint status = 0;
            if (FTDIhandler != IntPtr.Zero)
            {
                status = I2C_GetNumChannels(ref ch);
                if (status == 0 && ch == 2)
                {
                    return 0;
                }
                I2C_CloseChannel(FTDIhandler);
                FTDIhandler = IntPtr.Zero;
                return -1;
            }
            return -1;
        }
        static void Main(string[] args)
        {
            FTDI device = new FTDI();
            IntPtr FThandle ;
            int return_val = 0;
            uint channels = 0;
            uint i2c_status = 0;
            bool is_connected = false;
            int num_channels = 10;
            byte interrupt = 0;
            UInt32 size_transfered =0;
            num_channels = device.Init(channels);
            byte[] buffer = {0,0,0,0,0,1,0,0};
            
            device.setGPIOdir(1, 0); //Pin C1 // 1 will write
            //device.setGPIOdir(0, 0); // 0 will read
            //init code
            
           // channels = device.readGPIO( ref interrupt);
            i2c_status = device.I2Cwrite(0x34, 8,buffer , ref size_transfered,I2C_TRANSFER_OPTIONS_START_BIT);

            while(true)
            {


                //i2c_status = device.I2Cwrite(0x34, 8, buffer, ref size_transfered, I2C_TRANSFER_OPTIONS_START_BIT);
                device.I2Cread(0x34, 8, buffer, ref size_transfered, I2C_TRANSFER_OPTIONS_START_BIT);

                Thread.Sleep(1); 
                channels = device.readGPIO(ref interrupt);
               //Thread.Sleep(1);
               //channels = device.clearGPIO(1);
               //Thread.Sleep(1);
                if (interrupt == 0)
                {
                    //i2c_status = device.I2Cwrite(0x34, 8, buffer, ref size_transfered, I2C_TRANSFER_OPTIONS_START_BIT);
                    //Thread.Sleep(10);
                    device.I2Cread(0x34, 8, buffer, ref size_transfered, I2C_TRANSFER_OPTIONS_START_BIT);
                   // Thread.Sleep(10);
                }
                
            }

                Console.ReadLine();

        }
 
           
    }
   
   
}
  
  

