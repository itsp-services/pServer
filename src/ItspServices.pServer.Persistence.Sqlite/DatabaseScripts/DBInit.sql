-- -----------------------------------------------------
-- Table Roles
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Roles (
  ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Name VARCHAR(20) NOT NULL UNIQUE
);

-- -----------------------------------------------------
-- Table Users
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Users (
  ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  Username VARCHAR(45) NOT NULL UNIQUE,
  NormalizedUsername VARCHAR(45) UNIQUE,
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
  KeyData VARCHAR(364) NULL,
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
  Permission INT NOT NULL, -- 1=View, 2=Read, 3=Write, ...
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
  KeyData VARCHAR(364) NULL,
  PRIMARY KEY (UserID, PublicKey, DataID),
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

-- -----------------------------------------------------
-- View [Users Keys]
-- -----------------------------------------------------
CREATE VIEW IF NOT EXISTS [Users Keys] AS
SELECT Users.ID, Users.Username, Users.NormalizedUsername, Users.PasswordHash, Roles.Name As Role, PublicKeyNumber, KeyData, Active FROM Users
JOIN Roles ON Roles.ID=Users.RoleID
LEFT JOIN PublicKeys ON PublicKeys.UserID=Users.ID;

-- -----------------------------------------------------
-- Trigger add_normalized_username_after_insert
-- -----------------------------------------------------
CREATE TRIGGER IF NOT EXISTS add_normalized_username_after_insert
	AFTER INSERT ON Users
BEGIN
	UPDATE Users
	SET NormalizedUsername=upper(Username)
	WHERE Users.ID = NEW.ID;
END;

-- -----------------------------------------------------
-- Trigger set_normalized_username_after_update
-- -----------------------------------------------------
CREATE TRIGGER IF NOT EXISTS set_normalized_username_after_update
	AFTER UPDATE ON Users
BEGIN
	UPDATE Users
	SET NormalizedUsername=upper(Username)
	WHERE Users.ID = NEW.ID;
END;

-- -----------------------------------------------------
-- Trigger assign_public_key_number_after_insert
-- -----------------------------------------------------
CREATE TRIGGER IF NOT EXISTS assign_public_key_number_after_insert
	AFTER INSERT ON PublicKeys
BEGIN
	UPDATE PublicKeys
	SET PublicKeyNumber = ((SELECT max(PublicKeyNumber) FROM PublicKeys WHERE PublicKeys.UserID = NEW.UserID) + 1)
	WHERE PublicKeys.UserID = NEW.UserID AND PublicKeyNumber = NEW.PublicKeyNumber;
END;