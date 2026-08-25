import psycopg2

conn = psycopg2.connect(
    host="localhost",
    port=5432,
    dbname="EmployeeInfoSystem",
    user="postgres",
    password="Dasha20052005",
)
conn.autocommit = True
cur = conn.cursor()

cur.execute("ALTER TABLE requests DROP CONSTRAINT IF EXISTS requests_status_check")
cur.execute(
    "UPDATE requests SET status = 'assigned' WHERE status = 'new' AND manager_id IS NOT NULL"
)
cur.execute(
    "UPDATE requests SET status = 'accepted' WHERE status = 'new' AND manager_id IS NULL"
)
cur.execute("ALTER TABLE requests ALTER COLUMN status SET DEFAULT 'accepted'")
cur.execute(
    """
    ALTER TABLE requests
    ADD CONSTRAINT requests_status_check
    CHECK (status IN ('accepted', 'assigned', 'in_progress', 'done'))
    """
)

print("OK: requests_status_check updated")
cur.close()
conn.close()
