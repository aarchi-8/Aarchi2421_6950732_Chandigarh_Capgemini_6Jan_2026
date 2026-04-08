-- ============================================================
--  EventBookingDb  –  Seed Data Script
--  Run AFTER:  dotnet ef database update
--  Target:     SQL Server / LocalDB
-- ============================================================

USE EventBookingDb;
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
  (
    'Tech Summit 2025',
    'A full-day conference covering AI, cloud computing, and the future of software engineering. Keynotes from industry leaders and hands-on workshops.',
    '2025-09-15 09:00:00',
    'Hyderabad International Convention Centre',
    300,
    'Tech',
    1499.00
  ),
  (
    'Classical Carnatic Night',
    'An enchanting evening of Carnatic classical music featuring acclaimed vocalist Sudha Raghunathan and her ensemble.',
    '2025-08-22 18:30:00',
    'Music Academy, Chennai',
    500,
    'Music',
    799.00
  ),
  (
    'Startup Pitch Day',
    'Watch 20 early-stage startups pitch their ideas to a panel of top investors. Networking session follows.',
    '2025-07-30 10:00:00',
    'T-Hub, Hyderabad',
    200,
    'Conference',
    0.00
  ),
  (
    'React & .NET Workshop',
    'Hands-on 2-day workshop building a full-stack app with ASP.NET Core Web API and React. Bring your laptop!',
    '2025-08-05 09:00:00',
    'Capgemini Pune Campus',
    60,
    'Workshop',
    2499.00
  ),
  (
    'IPL Watch Party – Finals',
    'Catch the IPL final live on a giant screen with food, drinks, and 500 fellow fans. Limited seats!',
    '2025-06-01 19:00:00',
    'DLF Mall Food Court, Delhi',
    150,
    'Sports',
    299.00
  ),
  (
    'Open Mic Night',
    'Got a story, a poem, or a joke? Our monthly open mic welcomes all genres. Sign up at the door.',
    '2025-07-12 19:00:00',
    'The Hive Cafe, Bangalore',
    80,
    'General',
    199.00
  );
GO

PRINT 'Seed data inserted successfully – 6 events added.';
GO
