-- -----------------------------------------------------
-- Table Roles
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Roles (
  ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Name VARCHAR(20) NOT NULL
);

-- -----------------------------------------------------
-- Table Users
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Users (
  ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Username VARCHAR(45) NOT NULL,
  PasswordHash VARCHAR(45) NULL,
  RoleID NOT NULL,
  CONSTRAINT fk_User_Roles
    FOREIGN KEY (RoleID)
    REFERENCES Roles (ID)
);

-- -----------------------------------------------------
-- Table PublicKeys
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS PublicKeys (
  UserID INT NOT NULL,
  PublicKeyNumber INT NOT NULL,
  KeyData VARBINARY(256) NULL,
  Active TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (UserID, PublicKeyNumber),
  CONSTRAINT fk_PublicKeys_User
    FOREIGN KEY (UserID)
    REFERENCES Users (ID)
    ON DELETE CASCADE
    ON UPDATE CASCADE
);

-- -----------------------------------------------------
-- Table Folders
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Folders (
  ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  FolderName VARCHAR(45) NOT NULL,
  Parent INT NOT NULL,
  CONSTRAINT fk_Folder_Folder
    FOREIGN KEY (Parent)
    REFERENCES Folders (ID)
);

-- -----------------------------------------------------
-- Table ProtectedData
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS ProtectedData (
  ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Folder INT NOT NULL,
  DataName VARCHAR(45) NOT NULL,
  Data MEDIUMBLOB NULL,
  CONSTRAINT fk_ProtectedData_Folder
    FOREIGN KEY (Folder)
    REFERENCES Folders (ID)
);

-- -----------------------------------------------------
-- Table ProtectedDataPermissions
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS ProtectedDataPermissions (
  UserID INT NOT NULL,
  DataID INT NOT NULL,
  Permission VARCHAR(10) NOT NULL,
  PRIMARY KEY (UserID, DataID),
  CONSTRAINT fk_ProtectedDataPermissions_User
    FOREIGN KEY (UserID)
    REFERENCES Users (ID),
  CONSTRAINT fk_ProtectedDataPermissions_ProtectedData
    FOREIGN KEY (DataID)
    REFERENCES ProtectedData (ID)
);

-- -----------------------------------------------------
-- Table SymmetricKeys
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS SymmetricKeys (
  UserID INT NOT NULL,
  PublicKey INT NOT NULL,
  DataID INT NOT NULL,
  KeyData MEDIUMBLOB NULL,
  PRIMARY KEY (UserID, PublicKeyNumber, DataID),
  CONSTRAINT fk_SymmetricKeys_PublicKeys
    FOREIGN KEY (UserID , PublicKey)
    REFERENCES PublicKeys (UserID , PublicKeyNumber)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT fk_SymmetricKeys_ProtectedData
    FOREIGN KEY (DataID)
    REFERENCES ProtectedData (ID)
    ON DELETE CASCADE
    ON UPDATE CASCADE
);