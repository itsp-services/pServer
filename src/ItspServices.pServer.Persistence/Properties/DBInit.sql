-- -----------------------------------------------------
-- Table Roles
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Roles (
  Name VARCHAR(20) NOT NULL,
  PRIMARY KEY (Name)
);

-- -----------------------------------------------------
-- Table Users
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Users (
  UserID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Username VARCHAR(45) NOT NULL,
  PasswordHash VARCHAR(45) NULL,
  Role VARCHAR(20) NOT NULL,
  CONSTRAINT fk_User_Roles1
    FOREIGN KEY (Role)
    REFERENCES Roles (Name)
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
    REFERENCES Users (UserID)
    ON DELETE CASCADE
    ON UPDATE CASCADE
);

-- -----------------------------------------------------
-- Table Folders
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Folders (
  FolderID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  FolderName VARCHAR(45) NOT NULL,
  ParentFolderID INT NOT NULL,
  CONSTRAINT fk_Folder_Folder1
    FOREIGN KEY (ParentFolderID)
    REFERENCES Folders (FolderID)
);

-- -----------------------------------------------------
-- Table ProtectedData
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS ProtectedData (
  DataID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  ParentFolderID INT NOT NULL,
  DataName VARCHAR(45) NOT NULL,
  Data MEDIUMBLOB NULL,
  CONSTRAINT fk_ProtectedData_Folder1
    FOREIGN KEY (ParentFolderID)
    REFERENCES Folders (FolderID)
);

CREATE TABLE IF NOT EXISTS Permission 
(
  Name VARCHAR(10) NOT NULL PRIMARY KEY
);

-- -----------------------------------------------------
-- Table Users_manipulate_ProtectedData
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Users_manipulate_ProtectedData (
  UserID INT NOT NULL,
  DataID INT NOT NULL,
  Permission VARCHAR(10) NOT NULL,
  PRIMARY KEY (UserID, DataID),
  CONSTRAINT fk_Users_manipulate_ProtectedData_User
    FOREIGN KEY (UserID)
    REFERENCES Users (UserID),
  CONSTRAINT fk_Users_manipulate_ProtectedData_ProtectedData
    FOREIGN KEY (DataID)
    REFERENCES ProtectedData (DataID),
  CONSTRAINT fk_Users_manipulate_ProtectedData_Permission
    FOREIGN KEY (Permission)
    REFERENCES Permission (Name)
);

-- -----------------------------------------------------
-- Table SymmetricKeys
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS SymmetricKeys (
  UserID INT NOT NULL,
  PublicKeyNumber INT NOT NULL,
  DataID INT NOT NULL,
  KeyData MEDIUMBLOB NULL,
  PRIMARY KEY (UserID, PublicKeyNumber, DataID),
  CONSTRAINT fk_SymmetricKeys_PublicKeys1
    FOREIGN KEY (UserID , PublicKeyNumber)
    REFERENCES PublicKeys (UserID , PublicKeyNumber)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT fk_SymmetricKeys_ProtectedData1
    FOREIGN KEY (DataID)
    REFERENCES ProtectedData (DataID)
    ON DELETE CASCADE
    ON UPDATE CASCADE
);