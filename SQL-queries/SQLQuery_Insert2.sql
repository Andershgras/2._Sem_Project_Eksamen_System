-- ===========================
--  SUBJECTS
-- ===========================
INSERT INTO Subject (Name)
VALUES 
('Mathematics'),
('English'),
('Physics'),
('Computer Science');

-- ===========================
--  TEACHERS
-- ===========================
INSERT INTO Teacher (TeacherName, Mail)
VALUES
('Alice Johnson', 'alice.johnson@school.com'),
('Bob Smith', 'bob.smith@school.com'),
('Charlie Davis', 'charlie.davis@school.com');

-- ===========================
--  ROOMS
-- ===========================
INSERT INTO Room (Name)
VALUES 
('Room A1'),
('Room B2'),
('Lab C3');

-- ===========================
--  CLASSES
-- ===========================
INSERT INTO Class (ClassName)
VALUES
('1A'),
('1B'),
('2A');

-- ===========================
--  STUDENTS
-- ===========================
INSERT INTO Student (StudentName, Mail)
VALUES
('John Doe', 'john.doe@student.com'),
('Jane Smith', 'jane.smith@student.com'),
('Michael Brown', 'michael.brown@student.com'),
('Emily Davis', 'emily.davis@student.com');

-- ===========================
--  CLASS–SUBJECT
-- ===========================
-- Class 1A has Math & English
INSERT INTO ClassSubject (ClassID, SubjectID)
VALUES
(1, 1),  -- 1A - Mathematics
(1, 2),  -- 1A - English
(2, 3),  -- 1B - Physics
(3, 4);  -- 2A - Computer Science

-- ===========================
--  STUDENT–CLASS
-- ===========================
INSERT INTO StudentClass (ClassID, StudentID)
VALUES
(1, 1),  -- John Doe in 1A
(1, 2),  -- Jane Smith in 1A
(2, 3),  -- Michael Brown in 1B
(3, 4);  -- Emily Davis in 2A

-- ===========================
--  EXAMS
-- ===========================
INSERT INTO Exam (
    ExamName, SubjectID, ExamType, ExamStartDate, ExamEndDate, 
    DeliveryDate, ClassID, TeacherID, RoomID, TimeDuration, IsReExam
)
VALUES
('Math Midterm', 1, 'Written', '2025-03-10', '2025-03-10', '2025-03-11', 1, 1, 1, 90, 0),
('English Oral', 2, 'Oral', '2025-03-15', '2025-03-15', '2025-03-16', 1, 2, 2, 30, 0),
('Physics Lab Test', 3, 'Practical', '2025-03-20', '2025-03-20', '2025-03-21', 2, 3, 3, 120, 0);

-- ===========================
--  STUDENT–EXAM
-- ===========================
INSERT INTO StudentExam (StudentID, ExamID)
VALUES
(1, 1),  -- John in Math Midterm
(2, 1),  -- Jane in Math Midterm
(1, 2),  -- John in English Oral
(3, 3),  -- Michael in Physics Lab Test
(4, 3);  -- Emily in Physics Lab Test

-- ===========================
--  EXAMINATION–TEACHER
-- ===========================
INSERT INTO ExaminationTeacher (ExamID, TeacherID, Rolle)
VALUES
(1, 1, 'Main Examiner'),
(2, 2, 'Main Examiner'),
(2, 3, 'Censor'),
(3, 3, 'Main Examiner');
