-- ============================================================
--  EventBookingDb  –  Seed Data Script
--  Run AFTER:  dotnet ef database update
--  Target:     SQL Server / LocalDB
-- ============================================================

USE EventBookingDB;
GO

-- Clear existing seed data (safe to re-run)
DELETE FROM Bookings;
DELETE FROM Events;
DBCC CHECKIDENT ('Events',   RESEED, 0);
DBCC CHECKIDENT ('Bookings', RESEED, 0);
GO

-- ── Events ─────────────────────────────────────────────────
INSERT INTO Events (Title, Description, Date, Location, AvailableSeats, Category, TicketPrice)
VALUES
('Tech Summit 2025',
 'AI, cloud computing & software future with workshops.',
 '2025-09-15 09:00:00',
 'Hyderabad',
 300,
 'Tech',
 1499.00),

('Classical Carnatic Night',
 'Carnatic music evening with live performance.',
 '2025-08-22 18:30:00',
 'Chennai',
 500,
 'Music',
 799.00),

('Startup Pitch Day',
 'Startups pitching to investors + networking.',
 '2025-07-30 10:00:00',
 'Hyderabad',
 200,
 'Conference',
 0.00),

('React & .NET Workshop',
 '2-day full-stack workshop with ASP.NET Core & React.',
 '2025-08-05 09:00:00',
 'Pune',
 60,
 'Workshop',
 2499.00),

('IPL Watch Party – Finals',
 'Live IPL final screening with crowd & food.',
 '2025-06-01 19:00:00',
 'Delhi',
 150,
 'Sports',
 299.00),

('Open Mic Night',
 'Poetry, comedy & storytelling open mic.',
 '2025-07-12 19:00:00',
 'Bangalore',
 80,
 'General',
 199.00);
GO

PRINT 'Seed data inserted successfully – 6 events added.';
GO
