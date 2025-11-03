-- ===========================
--  SUBJECT
-- ===========================
CREATE TABLE Subject (
    SubjectID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(250) NOT NULL
);

-- ===========================
--  TEACHER
-- ===========================
CREATE TABLE Teacher (
    TeacherID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherName VARCHAR(250) NOT NULL,
    Mail VARCHAR(250)
);

-- ===========================
--  ROOM
-- ===========================
CREATE TABLE Room (
    RoomID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(250) NOT NULL
);

-- ===========================
--  CLASS
-- ===========================
CREATE TABLE Class (
    ClassID INT IDENTITY(1,1) PRIMARY KEY,
    ClassName VARCHAR(250) NOT NULL
);

-- ===========================
--  STUDENT
-- ===========================
CREATE TABLE Student (
    StudentID INT IDENTITY(1,1) PRIMARY KEY,
    StudentName VARCHAR(250) NOT NULL,
    Mail VARCHAR(250)
);

-- ===========================
--  CLASS–SUBJECT (junction)
-- ===========================
CREATE TABLE ClassSubject (
    ClassSubjectID INT IDENTITY(1,1) PRIMARY KEY,
    ClassID INT NOT NULL,
    SubjectID INT NOT NULL,
    CONSTRAINT FK_ClassSubject_Class 
        FOREIGN KEY (ClassID) REFERENCES Class(ClassID) ON DELETE CASCADE,
    CONSTRAINT FK_ClassSubject_Subject 
        FOREIGN KEY (SubjectID) REFERENCES Subject(SubjectID) ON DELETE CASCADE,
    CONSTRAINT UQ_ClassSubject UNIQUE (ClassID, SubjectID)
);

-- ===========================
--  STUDENT–CLASS (junction)
-- ===========================
CREATE TABLE StudentClass (
    StudentClassID INT IDENTITY(1,1) PRIMARY KEY,
    ClassID INT NOT NULL,
    StudentID INT NOT NULL,
    CONSTRAINT FK_StudentClass_Class 
        FOREIGN KEY (ClassID) REFERENCES Class(ClassID) ON DELETE CASCADE,
    CONSTRAINT FK_StudentClass_Student 
        FOREIGN KEY (StudentID) REFERENCES Student(StudentID) ON DELETE CASCADE,
    CONSTRAINT UQ_StudentClass UNIQUE (ClassID, StudentID)
);

-- ===========================
--  EXAM
-- ===========================
CREATE TABLE Exam (
    ExamID INT IDENTITY(1,1) PRIMARY KEY,
    ExamName VARCHAR(250) NOT NULL,
    SubjectID INT NULL,
    ExamType VARCHAR(100) NOT NULL,
    ExamStartDate DATE,
    ExamEndDate DATE,
    DeliveryDate DATE,
    ClassID INT NULL,
    TeacherID INT NULL,
    RoomID INT NULL,
    TimeDuration INT,
    ReExamDate DATE,
    IsReExam BIT DEFAULT 0,

    CONSTRAINT FK_Exam_Subject 
        FOREIGN KEY (SubjectID) REFERENCES Subject(SubjectID) ON DELETE SET NULL,
    CONSTRAINT FK_Exam_Class 
        FOREIGN KEY (ClassID) REFERENCES Class(ClassID) ON DELETE SET NULL,
    CONSTRAINT FK_Exam_Teacher 
        FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID) ON DELETE SET NULL,
    CONSTRAINT FK_Exam_Room 
        FOREIGN KEY (RoomID) REFERENCES Room(RoomID) ON DELETE SET NULL
);

-- ===========================
--  STUDENT–EXAM (junction)
-- ===========================
CREATE TABLE StudentExam (
    StudentExamID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    ExamID INT NOT NULL,
    -- (e.g., could add Grade, Result, etc.)
    CONSTRAINT FK_StudentExam_Student 
        FOREIGN KEY (StudentID) REFERENCES Student(StudentID) ON DELETE CASCADE,
    CONSTRAINT FK_StudentExam_Exam 
        FOREIGN KEY (ExamID) REFERENCES Exam(ExamID) ON DELETE CASCADE,
    CONSTRAINT UQ_StudentExam UNIQUE (StudentID, ExamID)
);

-- ===========================
--  EXAMINATION–TEACHER (junction)
-- ===========================
CREATE TABLE ExaminationTeacher (
    ExaminationTeacherID INT IDENTITY(1,1) PRIMARY KEY,
    ExamID INT NOT NULL,
    TeacherID INT NOT NULL,
    Rolle VARCHAR(100),
    CONSTRAINT FK_ExaminationTeacher_Exam 
        FOREIGN KEY (ExamID) REFERENCES Exam(ExamID) ON DELETE CASCADE,
    CONSTRAINT FK_ExaminationTeacher_Teacher 
        FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID) ON DELETE CASCADE,
    CONSTRAINT UQ_ExaminationTeacher UNIQUE (ExamID, TeacherID)
);
