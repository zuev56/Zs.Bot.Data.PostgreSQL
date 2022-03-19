--
CREATE OR REPLACE FUNCTION public.reset_update_date()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
   NEW.update_date = now(); 
   RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.reset_update_date()
    OWNER TO postgres;

COMMENT ON FUNCTION public.reset_update_date()
    IS 'Общая триггерная функция для обовления поля update_date';

CREATE TRIGGER chats_reset_update_date
    BEFORE UPDATE 
    ON bot.chats
    FOR EACH ROW
    EXECUTE FUNCTION public.reset_update_date();

CREATE TRIGGER messages_reset_update_date
    BEFORE UPDATE 
    ON bot.messages
    FOR EACH ROW
    EXECUTE FUNCTION public.reset_update_date();

CREATE TRIGGER users_reset_update_date
    BEFORE UPDATE 
    ON bot.users
    FOR EACH ROW
    EXECUTE FUNCTION public.reset_update_date();

--
CREATE OR REPLACE FUNCTION bot.commands_new_command_correct()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $BODY$
BEGIN
    NEW.id = lower(trim(NEW.id));
    NEW.id = replace(NEW.id, '/', '' );
    NEW.id = '/' || NEW.id;
    
    NEW.group = lower(trim(NEW.group));
    NEW.group = replace(NEW.group, 'cmdgroup', '' );
    NEW.group = NEW.group || 'CmdGroup';

    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION bot.commands_new_command_correct()
    OWNER TO postgres;

CREATE TRIGGER commands_new_command_correct
    BEFORE INSERT OR UPDATE 
    ON bot.commands
    FOR EACH ROW
    EXECUTE FUNCTION bot.commands_new_command_correct();