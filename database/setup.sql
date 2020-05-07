CREATE DATABASE $(DB_NAME);
GO
USE $(DB_NAME);
GO
CREATE LOGIN $(DB_USERNAME) WITH PASSWORD = '$(DB_PASSWORD)'
GO
CREATE USER $(DB_USERNAME) FOR LOGIN $(DB_USERNAME)
GO
ALTER ROLE db_datareader ADD MEMBER $(DB_USERNAME);
ALTER ROLE db_datawriter ADD MEMBER $(DB_USERNAME);
ALTER ROLE db_ddladmin ADD MEMBER $(DB_USERNAME);
GO
-- Disable sa login for security
ALTER LOGIN [sa] DISABLE;
GO

CREATE TABLE departments
(
  [id] int not null identity,
  [name] nvarchar(64) not null,
  PRIMARY KEY ([id])
);

CREATE TABLE users
(
  [id] int not null identity,
  [department_id] int null,
  [username] nvarchar(32) not null,
  [password] nvarchar(255) not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [users] ADD CONSTRAINT [FK_users_department_id]
  FOREIGN KEY ([department_id])
  REFERENCES departments ([id])
  ON DELETE CASCADE;

CREATE TABLE students
(
  [id] nvarchar(10) not null,
  [full_name] nvarchar(255) not null,
  PRIMARY KEY ([id])
);

CREATE TABLE course_infos
(
  [id] int not null identity,
  [department_id] int not null,
  [code] nvarchar(7) not null,
  [name] nvarchar(64) not null,
  [credit] int not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [course_infos] ADD CONSTRAINT [FK_course_infos_department_id]
  FOREIGN KEY ([department_id])
  REFERENCES departments ([id]);

CREATE TABLE courses
(
  [id] int not null identity,
  [course_info_id] int not null,
  [semester] nvarchar(10) not null CHECK (semester IN('Fall', 'Spring', 'Summer')),
  [year] int not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [courses] ADD CONSTRAINT [FK_courses_course_info_id]
  FOREIGN KEY ([course_info_id])
  REFERENCES course_infos ([id]);

CREATE TABLE course_results
(
  [id] int not null identity,
  [student_id] nvarchar(10) not null,
  [course_id] int not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [course_results] ADD CONSTRAINT [FK_course_results_student_id]
  FOREIGN KEY ([student_id])
  REFERENCES students ([id]);

ALTER TABLE [course_results] ADD CONSTRAINT [FK_course_results_course_id]
  FOREIGN KEY ([course_id])
  REFERENCES courses ([id]);

CREATE TABLE assignments
(
  [id] int not null identity,
  [course_id] int not null,
  [type] nvarchar(10) not null CHECK ([type] IN('Midterm', 'Final', 'Project', 'LabExam', 'Assignment', 'Other')),
  [weight] decimal(3, 3) not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [assignments] ADD CONSTRAINT [FK_assignments_course_id]
  FOREIGN KEY ([course_id])
  REFERENCES courses ([id]);

CREATE TABLE assignment_results
(
  [id] int not null identity,
  [assignment_id] int not null,
  [course_result_id] int not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [assignment_results] ADD CONSTRAINT [FK_assignment_results_course_result_id]
  FOREIGN KEY ([course_result_id])
  REFERENCES course_results ([id]);

ALTER TABLE [assignment_results] ADD CONSTRAINT [FK_assignment_results_assignment_id]
  FOREIGN KEY ([assignment_id])
  REFERENCES assignments ([id]);

CREATE TABLE assignment_tasks
(
  [id] int not null identity,
  [assignment_id] int not null,
  [number] int not null,
  [weight] decimal(3, 3) not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [assignment_tasks] ADD CONSTRAINT [FK_assignment_tasks_assignment_id]
  FOREIGN KEY ([assignment_id])
  REFERENCES assignments ([id]);

CREATE TABLE outcomes
(
  [id] int not null identity,
  [code] tinyint not null,
  [description] nvarchar(255) not null,
  [type] nvarchar(8) not null CHECK ([type] IN('learning', 'program')),
  [course_info_id] int null,
  [department_id] int null,
  PRIMARY KEY ([id])
);

ALTER TABLE [outcomes] ADD CONSTRAINT [FK_outcomes_department_id]
  FOREIGN KEY ([department_id])
  REFERENCES departments ([id])
  ON DELETE CASCADE;

ALTER TABLE [outcomes] ADD CONSTRAINT [FK_outcomes_course_info_id]
  FOREIGN KEY ([course_info_id])
  REFERENCES course_infos ([id])
  ON DELETE CASCADE;

CREATE TABLE assignment_task_outcome
(
  [assignment_task_id] int not null,
  [outcome_id] int not null,
  PRIMARY KEY ([assignment_task_id], [outcome_id])
);

ALTER TABLE [assignment_task_outcome] ADD CONSTRAINT [FK_assignment_task_outcome_assignment_task_id]
  FOREIGN KEY ([assignment_task_id])
  REFERENCES assignment_tasks ([id])
  ON DELETE CASCADE;

ALTER TABLE [assignment_task_outcome] ADD CONSTRAINT [FK_assignment_task_outcome_outcome_id]
  FOREIGN KEY ([outcome_id])
  REFERENCES outcomes ([id])
  ON DELETE CASCADE;

CREATE TABLE assignment_task_results
(
  [id] int not null identity,
  [assignment_task_id] int not null,
  [assignment_result_id] int not null,
  [grade] decimal(5, 2) not null,
  PRIMARY KEY ([id])
);

ALTER TABLE [assignment_task_results] ADD CONSTRAINT [FK_assignment_task_results_assignment_task_id]
  FOREIGN KEY ([assignment_task_id])
  REFERENCES assignment_tasks ([id]);

ALTER TABLE [assignment_task_results] ADD CONSTRAINT [FK_assignment_task_results_assignment_result_id]
  FOREIGN KEY ([assignment_result_id])
  REFERENCES assignment_results ([id]);

-- TRIGGERS

GO
CREATE TRIGGER [DELETE_departments]
   ON [departments]
   INSTEAD OF DELETE
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [course_infos] WHERE department_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [departments] WHERE id IN (SELECT id
  FROM DELETED)
END

GO
CREATE TRIGGER [DELETE_course_infos]
   ON [course_infos]
   INSTEAD OF DELETE
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [courses] WHERE course_info_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [course_infos] WHERE id IN (SELECT id
  FROM DELETED)
END

GO
CREATE TRIGGER [DELETE_courses]
   ON [courses]
   INSTEAD OF DELETE
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [assignments] WHERE course_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [course_results] WHERE course_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [courses] WHERE id IN (SELECT id
  FROM DELETED)
END

GO
CREATE TRIGGER [DELETE_course_results]
   ON [course_results]
   INSTEAD OF DELETE
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [assignment_results] WHERE course_result_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [course_results] WHERE id IN (SELECT id
  FROM DELETED)
END

GO
CREATE TRIGGER [DELETE_assignments]
   ON [assignments]
   INSTEAD OF DELETE
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [assignment_results] WHERE assignment_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [assignment_tasks] WHERE assignment_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [assignments] WHERE id IN (SELECT id
  FROM DELETED)
END

GO
CREATE TRIGGER [DELETE_assignment_results]
   ON dbo.[assignment_results]
   INSTEAD OF DELETE
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [assignment_task_results] WHERE assignment_task_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [assignment_results] WHERE id IN (SELECT id
  FROM DELETED)
END

GO
CREATE TRIGGER [DELETE_assignment_tasks]
   ON dbo.[assignment_tasks]
   INSTEAD OF DELETE
AS 
BEGIN
  SET NOCOUNT ON;
  DELETE FROM [assignment_task_results] WHERE assignment_task_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [assignment_task_outcome] WHERE assignment_task_id IN (SELECT id
  FROM DELETED)
  DELETE FROM [assignment_tasks] WHERE id IN (SELECT id
  FROM DELETED)
END

GO
