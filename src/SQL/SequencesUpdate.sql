-- After migrations seeding we add Chat and User 
-- Does not work with id = 1 and id's autoincrement (bug)
SELECT nextval(pg_get_serial_sequence('bot.chats', 'id'));
SELECT nextval(pg_get_serial_sequence('bot.users', 'id'));