-- De nederstående Queries er skrevet ved hjælp af ChatGPT

-- ================================================
-- Database: Exam Management System
-- Safe Version: Cascades only on junction tables
-- ================================================

-- Drop tables if they exist (for re-runs)
DROP TABLE IF EXISTS EksaminationsUndervise;
DROP TABLE IF EXISTS ELEV_EKSAMEN;
DROP TABLE IF EXISTS ELEV_HOLD;
DROP TABLE IF EXISTS HOLD_FAG;
DROP TABLE IF EXISTS Exam;
DROP TABLE IF EXISTS Lokale;
DROP TABLE IF EXISTS Underviser;
DROP TABLE IF EXISTS Elever;
DROP TABLE IF EXISTS Fag;
DROP TABLE IF EXISTS Hold;

-- ================================================
-- Base Tables
-- ================================================

CREATE TABLE Hold (
    HoldId INT IDENTITY(1,1) PRIMARY KEY,
    HoldNavn VARCHAR(250)
);

CREATE TABLE Fag (
    FagID INT IDENTITY(1,1) PRIMARY KEY,
    Navn VARCHAR(250)
);

CREATE TABLE Lokale (
    LokaleId INT IDENTITY(1,1) PRIMARY KEY,
    Navn VARCHAR(250)
);

CREATE TABLE Underviser (
    UnderviserID INT IDENTITY(1,1) PRIMARY KEY,
    UnderviserNavn VARCHAR(250),
    Mail VARCHAR(250)
);

CREATE TABLE Elever (
    ElevId INT IDENTITY(1,1) PRIMARY KEY,
    ElevNavn VARCHAR(250),
    Mail VARCHAR(250)
);

-- ================================================
-- Exam Table
-- No cascading here; FKs set to NULL if parent deleted
-- ================================================

CREATE TABLE Exam (
    ExamId INT IDENTITY(1,1) PRIMARY KEY,
    ExamName VARCHAR(250),
    FagID INT NULL,
    ExamType VARCHAR(250),
    ExamDate DATETIME2,
    HoldID INT NULL,
    UnderviserID INT NULL,
    LokaleID INT NULL,
    TimeDuration INT, -- in minutes
    ReEksamenDato DATETIME2,
    CONSTRAINT FK_Exam_Fag FOREIGN KEY (FagID) REFERENCES Fag(FagID) ON DELETE SET NULL,
    CONSTRAINT FK_Exam_Hold FOREIGN KEY (HoldID) REFERENCES Hold(HoldId) ON DELETE SET NULL,
    CONSTRAINT FK_Exam_Underviser FOREIGN KEY (UnderviserID) REFERENCES Underviser(UnderviserID) ON DELETE SET NULL,
    CONSTRAINT FK_Exam_Lokale FOREIGN KEY (LokaleID) REFERENCES Lokale(LokaleId) ON DELETE SET NULL
);

-- ================================================
-- Junction Tables (Composite Keys)
-- Cascades kept only here
-- ================================================

CREATE TABLE ELEV_EKSAMEN (
    ExamID INT,
    ElevID INT,
    PRIMARY KEY (ExamID, ElevID),
    CONSTRAINT FK_ElevExam_Exam FOREIGN KEY (ExamID) REFERENCES Exam(ExamId) ON DELETE CASCADE,
    CONSTRAINT FK_ElevExam_Elev FOREIGN KEY (ElevID) REFERENCES Elever(ElevId) ON DELETE CASCADE
);

CREATE TABLE ELEV_HOLD (
    HoldID INT,
    ElevID INT,
    PRIMARY KEY (HoldID, ElevID),
    CONSTRAINT FK_ElevHold_Hold FOREIGN KEY (HoldID) REFERENCES Hold(HoldId) ON DELETE CASCADE,
    CONSTRAINT FK_ElevHold_Elev FOREIGN KEY (ElevID) REFERENCES Elever(ElevId) ON DELETE CASCADE
);

CREATE TABLE HOLD_FAG (
    HoldID INT,
    FagID INT,
    PRIMARY KEY (HoldID, FagID),
    CONSTRAINT FK_HoldFag_Hold FOREIGN KEY (HoldID) REFERENCES Hold(HoldId) ON DELETE CASCADE,
    CONSTRAINT FK_HoldFag_Fag FOREIGN KEY (FagID) REFERENCES Fag(FagID) ON DELETE CASCADE
);

CREATE TABLE EksaminationsUndervise (
    ExamID INT,
    UnderviserID INT,
    Rolle VARCHAR(250),
    PRIMARY KEY (ExamID, UnderviserID),
    CONSTRAINT FK_EksaminationsUndervise_Exam FOREIGN KEY (ExamID) REFERENCES Exam(ExamId) ON DELETE CASCADE,
    CONSTRAINT FK_EksaminationsUndervise_Underviser FOREIGN KEY (UnderviserID) REFERENCES Underviser(UnderviserID) ON DELETE NO ACTION
);
