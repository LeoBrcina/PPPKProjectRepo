CREATE TABLE Patients (
    PatientID SERIAL PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    OIB VARCHAR(11) UNIQUE NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender CHAR(1) NOT NULL CHECK (Gender IN ('M', 'F'))
);

CREATE TABLE Diseases (
    DiseaseID SERIAL PRIMARY KEY,
    DiseaseName TEXT NOT NULL
);

CREATE TABLE Medicines (
    MedicineID SERIAL PRIMARY KEY,
    MedicineName TEXT NOT NULL
);

CREATE TABLE MedicalRecords (
    RecordID SERIAL PRIMARY KEY,
    PatientID INT NOT NULL REFERENCES Patients(PatientID) ON DELETE CASCADE,
    StartDate DATE NOT NULL,
    EndDate DATE,
    DiseaseID INT NOT NULL REFERENCES Diseases(DiseaseID) ON DELETE CASCADE
);

CREATE TABLE AppointmentTypes (
    AppointmentTypeID SERIAL PRIMARY KEY,
    AppointmentTypeName TEXT NOT NULL UNIQUE
);

CREATE TABLE Appointments (
    AppointmentID SERIAL PRIMARY KEY,
    PatientID INT NOT NULL REFERENCES Patients(PatientID) ON DELETE CASCADE,
    AppointmentDate TIMESTAMP NOT NULL,
    AppointmentTypeID INT NOT NULL REFERENCES AppointmentTypes(AppointmentTypeID) ON DELETE SET NULL
);

CREATE TABLE Prescriptions (
    PrescriptionID SERIAL PRIMARY KEY,
    PatientID INT NOT NULL REFERENCES Patients(PatientID) ON DELETE CASCADE,
    MedicineID INT NOT NULL REFERENCES Medicines(MedicineID) ON DELETE CASCADE
);

CREATE TABLE Attachments (
    AttachmentID SERIAL PRIMARY KEY,
    AppointmentID INT NOT NULL REFERENCES Appointments(AppointmentID) ON DELETE CASCADE,
    FileName VARCHAR(255) NOT NULL,
    FilePath VARCHAR(255) NOT NULL,
    UploadedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
