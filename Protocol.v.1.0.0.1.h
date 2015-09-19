#ifndef __pg_Protocols_H_
#define __pg_Protocols_H_
//#include "const.h"
#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

#if defined(WIN32)
typedef unsigned char uint8_t;
typedef unsigned short uint16_t;
#endif

const uint8_t c_btPacketSizeInBytes = 22; 
#define SIZE_ASSERT(type, size) \
	struct staticSizeAssert_##type {char ___error___sizeof_##type##_is_not_##size[(sizeof(type) == size) - 1];}

typedef enum {
	pgLabelInfoPacket = 0x00, // Label => Server.
	pgLabelBroadcastPacket = 0x01, // Label => All.
	pgLabelControlPacket = 0x80, // Server => Label.

	pgWorkerDeviceInfoPacket = 0x40, // Worker Device (clock) => Server.
	pgWorkerDeviceControlPacket = 0x81, // Server => Worker Device (clock).
	pgWorkerDeviceLabelPacket = 0x41, // Worker Device (clock) => Server.
	pgWorkerDeviceMessagePacket = 0x82, // Server => Worker Device (clock).

	pgCookStationSignalPacket = 0x20, // Cook station => Server.
	pgCookStationControlPacket = 0x83, // Server => Cook station.
}PACKET_TYPES;

#pragma pack (push, 1)
	
typedef struct {
	uint8_t btHardwareMajor;
	uint8_t btHardwareMinor;
	uint8_t btSoftwareMajor;
	uint8_t btSoftwareMinor;
	uint16_t wBuild;
}VesrsionInfo;
// пакет от Метки к Серверу 
// Пакет информационный
typedef struct {
	uint16_t AdressSrc;
	VesrsionInfo version;
	uint8_t btPacketType; // == pgLabelInfoPacket
	uint16_t wBatteryVoltage;
	uint8_t btState; // 0 - passive, 1 - active
	uint8_t reserved[10];
}LabelInfoPacket ;

// пакет от Метки ко Всем 
// Пакет информационный
typedef struct {
	uint16_t AdressSrc;
	VesrsionInfo version;
	uint8_t btPacketType; // == pgLabelBroadcastPacket
	uint8_t reserved[12];
}LabelBroadcastPacket;

//SIZE_ASSERT(LabelInfoPacket, c_btPacketSizeInBytes);

// пакет от Сервера к Метки 
// Пакет конфигурационный
typedef struct {
	uint16_t AdressDst;
	uint8_t btPacketType; // == pgLabelControlPacket
	uint8_t btState; // 0 - passive, 1 - active
	uint8_t reserved[18];
}LabelControlPacket ;
//SIZE_ASSERT(LabelControlPacket, c_btPacketSizeInBytes);

// пакет от Часиков к Серверу 
// Пакет информационный о состояние Часиков

typedef struct {
	uint16_t AdressSrc;
	VesrsionInfo version;
	uint8_t btPacketType; // == pgWorkerDeviceInfoPacket
	uint16_t wBatteryVoltage;
	uint8_t btChanging; // 1 - charging, 0 - dischanging.
	uint8_t btLabelMinSignalLevel;
	uint8_t btLabelObserveTimeoutSec;
	uint8_t reserved[8];
}WorkerDeviceInfoPacket ;
//SIZE_ASSERT(WorkerDeviceInfoPacket, c_btPacketSizeInBytes);

// пакет от Сервера к Часикам 
// Пакет конфигурационный

typedef struct {
	uint16_t AdressDst;
	uint8_t btPacketType; // == pgWorkerDeviceControlPacket
	uint8_t btLabelMinSignalLevel;
	uint8_t btLabelObserveTimeoutSec;
	uint8_t reserved[17];
}WorkerDeviceControlPacket ;
//SIZE_ASSERT(WorkerDeviceControlPacket, c_btPacketSizeInBytes);

// пакет от Часиков к Серверу 
// Пакет О том что увидили метку

typedef struct {
	uint16_t AdressSrc;
	VesrsionInfo version;//6 BYTE
	uint8_t btPacketType; // == pgWorkerDeviceLabelPacket
	uint16_t btLabelId;
	uint8_t btSignalPower;
	uint8_t reserved[10];
}WorkerDeviceLabelPacket;
//SIZE_ASSERT(WorkerDeviceLabelPacket, c_btPacketSizeInBytes);

// пакет от Сервера к Часикам 
// Пакет с сообщением
typedef struct {
	uint16_t AdressDst;
	uint8_t btPacketType; // == pgWorkerDeviceMessagePacket
	uint8_t btMessageMask;
	uint8_t strMessage[18]; // null terminated, cp1251 encoded string.
}WorkerDeviceMessagePacket ;



//SIZE_ASSERT(WorkerDeviceMessagePacket, c_btPacketSizeInBytes);
// пакет от станции повара к Серверу 
// Пакет О нажатой кнопки
typedef struct {
	uint16_t AdressSrc;
	VesrsionInfo version;
	uint8_t btPacketType; // == pgCookStationSignalPacket
	uint8_t btButtonId;
	uint8_t reserved[12];
}CookStationSignalPacket ;
//SIZE_ASSERT(CookStationSignalPacket, c_btPacketSizeInBytes);
// пакет от Сервера к станции повара  
// Пакет О подсветки нажатой кнопки
typedef struct {
	uint16_t AdressDst;
	uint8_t btPacketType; // == pgCookStationControlPacket
	uint8_t btButtonId;
	uint8_t btButtonState; // 1 - active (pressed), 0 - inactive.
	uint8_t reserved[17];
}CookStationControlPacket ;

//SIZE_ASSERT(CookStationControlPacket, c_btPacketSizeInBytes);

#pragma pack (pop)

#ifdef __cplusplus
}
#endif // __cplusplus

#endif // __pg_Protocols_H_