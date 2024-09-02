-- Shooting range
CREATE TABLE IF NOT EXISTS ShootingRange(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name VARCHAR(30) UNIQUE NOT NULL,
    Address VARCHAR(100),
    Latitude DOUBLE,
    Longitude DOUBLE,
    IsMarkedAsFavourite BOOLEAN NOT NULL,
    BackgroundImgPath VARCHAR(255)
);

-- Sub range
CREATE TABLE IF NOT EXISTS SubRange(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ShootingRange_ID INT NOT NULL,
    RangeInMeters INTEGER NOT NULL,
    Altitude DOUBLE,
    DirectionToNorthDegrees INT,
    VerticalFiringOffsetDegrees INT,
    Prefix VARCHAR(1),
    NotesPath VARCHAR(255),
    
    FOREIGN KEY (ShootingRange_ID) REFERENCES ShootingRange(ID)
);

-- Triggers
CREATE TRIGGER IF NOT EXISTS DeleteSubRanges
        BEFORE DELETE
            ON ShootingRange
      FOR EACH ROW
BEGIN
    DELETE FROM SubRange
          WHERE SubRange.ShootingRange_ID = OLD.ID;
END;

-- Countries
CREATE TABLE IF NOT EXISTS Country(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name VARCHAR(50) NOT NULL,
    Code VARCHAR(2) NOT NULL UNIQUE
);

--Manfuacturer Type
CREATE TABLE IF NOT EXISTS ManufacturerType(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name VARCHAR(30) NOT NULL UNIQUE
);

-- Manufacturer
CREATE TABLE IF NOT EXISTS Manufacturer(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Country_ID INTEGER NOT NULL,
    ManufacturerType_ID INTEGER NOT NULL,
    Name VARCHAR(30) NOT NULL UNIQUE,
    
    FOREIGN KEY (Country_ID) REFERENCES Country(ID),
    FOREIGN KEY (ManufacturerType_ID) REFERENCES ManufacturerType(ID)
);

-- Firearm Caliber
CREATE TABLE IF NOT EXISTS FirearmCaliber(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Caliber VARCHAR(30) NOT NULL UNIQUE
);

-- FirearmType
CREATE TABLE IF NOT EXISTS FirearmType(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    TypeName VARCHAR(30) NOT NULL UNIQUE
);

-- Firearm sight click type
CREATE TABLE IF NOT EXISTS SightClickType(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ClickTypeName VARCHAR(10) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS SightReticle(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name VARCHAR(50) NOT NULL UNIQUE,
    BackgroundImgPath VARCHAR(255)
);

-- Firearm setting
CREATE TABLE IF NOT EXISTS FirearmSightSetting(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FirearmSight_ID INT NOT NULL,
    Distance INT,
    ElevationValue INT,
    WindageValue INT,
    
    FOREIGN KEY (FirearmSight_ID) REFERENCES FirearmSight(ID)
);

-- Optic
CREATE TABLE IF NOT EXISTS FirearmSight(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ClickType_ID INT NOT NULL,
    Manufacturer_ID INT NOT NULL,
    SightReticle_ID INT NOT NULL,
    Name VARCHAR(30) NOT NULL,
    OneClickValue DECIMAL(10,2) NOT NULL,
    
    FOREIGN KEY (ClickType_ID) REFERENCES SightClickType(ID),
    FOREIGN KEY (Manufacturer_ID) REFERENCES Manufacturer(ID),
    FOREIGN KEY (SightReticle_ID) REFERENCES SightReticle(ID)
);

-- Triggers
CREATE TRIGGER IF NOT EXISTS DeleteFirearmSightSettings
        BEFORE DELETE
            ON FirearmSight
      FOR EACH ROW
BEGIN
    DELETE FROM FirearmSightSetting
          WHERE FirearmSightSetting.FirearmSight_ID = OLD.ID;
END;

-- Firearm
CREATE TABLE IF NOT EXISTS Firearm(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    FirearmType_ID INTEGER NOT NULL,
    Manufacturer_ID INTEGER NOT NULL,
    Caliber_ID INTEGER NOT NULL,
    FirearmSight_ID INTEGER NOT NULL,
    Name VARCHAR(30) NOT NULL,
    Model VARCHAR(30),
    SerialNumber VARCHAR(100),
    TotalLengthMm DECIMAL(10,2),
    BarrelLengthInch DECIMAL(10,2),
    RateOfTwist VARCHAR(10),
    Weight DECIMAL(10,2),
    HandednessForLeft BOOLEAN,
     
    FOREIGN KEY (FirearmType_ID) REFERENCES FirearmType(ID),
    FOREIGN KEY (Manufacturer_ID) REFERENCES Manufacturer(ID),
    FOREIGN KEY (Caliber_ID) REFERENCES FirearmCaliber(ID),
    FOREIGN KEY (FirearmSight_ID) REFERENCES FirearmSight(ID)
);

CREATE TABLE IF NOT EXISTS Weather(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Clouds VARCHAR(50),
    Temperature DOUBLE,
    Pressure INTEGER,
    Humidity INTEGER,
    WindSpeed INTEGER,
    DirectionDegrees INTEGER
);

-- Shooting session
CREATE TABLE IF NOT EXISTS ShootingRecord(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ShootingRange_ID INTEGER NOT NULL,
    SubRange_ID INTEGER NOT NULL,
    Firearm_ID INTEGER NOT NULL,
    Weather_ID INTEGER,
    ElevationClicksOffset INTEGER NOT NULL,
    WindageClicksOffset INTEGER NOT NULL,
    Distance INTEGER NOT NULL,
    TimeTaken INTEGER NOT NULL,

    
    FOREIGN KEY (ShootingRange_ID) REFERENCES ShootingRange(ID),
    FOREIGN KEY (SubRange_ID) REFERENCES SubRange(ID),
    FOREIGN KEY (Firearm_ID) REFERENCES Firearm(ID),
    FOREIGN KEY (Weather_ID) REFERENCES Weather(ID)
);

-- Shooting session image
CREATE TABLE IF NOT EXISTS ShootingRecordImage(
    ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ShootingRecord_ID INT NOT NULL,
    RelativePathFromAppData VARCHAR(100) NOT NULL,
    
    FOREIGN KEY (ShootingRecord_ID) REFERENCES ShootingRecord(ID)
);

-- Triggers
CREATE TRIGGER IF NOT EXISTS DeleteShootingRecordImages
        BEFORE DELETE
            ON ShootingRecord
      FOR EACH ROW
BEGIN
    DELETE FROM ShootingRecordImagePath
          WHERE ShootingRecordImagePath.ShootingRecord_ID = OLD.ID;
END;