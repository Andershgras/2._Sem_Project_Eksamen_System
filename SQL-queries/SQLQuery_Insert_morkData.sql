-- ================================================
-- Populate Base Tables
-- ================================================

-- Holds (Classes)
INSERT INTO Hold (HoldNavn) VALUES
('Hold A'),
('Hold B');

-- Subjects (Fag)
INSERT INTO Fag (Navn) VALUES
('Matematik'),
('Fysik'),
('Programmering');

-- Rooms (Lokale)
INSERT INTO Lokale (Navn) VALUES
('Lokale 101'),
('Lokale 102');

-- Teachers (Underviser)
INSERT INTO Underviser (UnderviserNavn, Mail) VALUES
('Peter Hansen', 'peter.hansen@example.com'),
('Anna Jensen', 'anna.jensen@example.com');

-- Students (Elever)
INSERT INTO Elever (ElevNavn, Mail) VALUES
('Lars Nielsen', 'lars.nielsen@example.com'),
('Maria Pedersen', 'maria.pedersen@example.com'),
('Jonas Sørensen', 'jonas.sorensen@example.com');

-- ================================================
-- Populate Exam Table
-- ================================================

-- Exams
INSERT INTO Exam (ExamName, FagID, ExamType, ExamDate, HoldID, UnderviserID, LokaleID, TimeDuration, ReEksamenDato) VALUES
('Matematik Eksamen 1', 1, 'Skriftlig', '2025-06-10 09:00', 1, 1, 1, 120, '2025-08-10 09:00'),
('Fysik Eksamen 1', 2, 'Skriftlig', '2025-06-12 09:00', 1, 2, 2, 90, '2025-08-12 09:00'),
('Programmering Eksamen 1', 3, 'Praktisk', '2025-06-15 13:00', 2, 2, 1, 180, '2025-08-15 13:00');

-- ================================================
-- Populate Junction Tables
-- ================================================

-- Student-Hold
INSERT INTO ELEV_HOLD (HoldID, ElevID) VALUES
(1, 1),
(1, 2),
(2, 3);

-- Hold-Subject
INSERT INTO HOLD_FAG (HoldID, FagID) VALUES
(1, 1),
(1, 2),
(2, 3);

-- Student-Exam
INSERT INTO ELEV_EKSAMEN (ExamID, ElevID) VALUES
(1, 1),
(1, 2),
(2, 1),
(2, 2),
(3, 3);

-- Exam-Teacher (Role table)
INSERT INTO EksaminationsUndervise (ExamID, UnderviserID, Rolle) VALUES
(1, 1, 'Sensor'),
(2, 2, 'Sensor'),
(3, 2, 'Sensor');
