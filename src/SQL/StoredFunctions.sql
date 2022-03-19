
-- Get permissions for a specific user role
CREATE OR REPLACE FUNCTION bot.sf_get_permission_array(IN user_role_id_ character varying)
    RETURNS text[]
    LANGUAGE 'plpgsql'
    VOLATILE
    PARALLEL UNSAFE
    COST 100
    
AS $BODY$
BEGIN
    RETURN array(
        with json_permissions as (select ur.permissions as col
                                    from bot.user_roles ur 
                                   where upper(ur.id) = upper(user_role_id_)),
              row_Permissions as (select json_array_elements_text(col) as permissions
                                    from json_permissions)
        select permissions
          from row_Permissions
    );
END;
$BODY$;

ALTER FUNCTION bot.sf_get_permission_array(character varying)
    OWNER TO postgres;

COMMENT ON FUNCTION bot.sf_get_permission_array(character varying)
    IS 'Returns permission array for the role';




-- Get list of awailable functions for a specific user role
CREATE OR REPLACE FUNCTION bot.sf_cmd_get_help(IN user_role_id_ character varying)
    RETURNS text
    LANGUAGE 'plpgsql'
    VOLATILE
    PARALLEL UNSAFE
    COST 100
    
AS $BODY$
DECLARE
    role_permissions TEXT[];
BEGIN
    role_permissions := (select bot.sf_get_permission_array(user_role_id_));
    RAISE NOTICE 'role_permissions: %', role_permissions;
   
    IF 'ALL' = any(upper(role_permissions::text)::text[]) THEN
        RETURN (select string_agg((c.id || ' - ' || c.description), E'\n') 
                  from bot.commands c);
    ELSE
        RETURN (select string_agg((c.id || ' - ' || c.description), E'\n') 
                  from bot.commands c
                 where c.group = any(role_permissions));	
    END IF;	   
END;
$BODY$;

ALTER FUNCTION bot.sf_cmd_get_help(character varying)
    OWNER TO postgres;

COMMENT ON FUNCTION bot.sf_cmd_get_help(character varying)
    IS 'Returns help to features available for the role';




-- Get statistics of specific chat
CREATE OR REPLACE FUNCTION bot.sf_get_chat_statistics(IN _chat_id integer,IN _users_limit integer,IN _from_date timestamp with time zone,IN _to_date timestamp with time zone DEFAULT now())
    RETURNS TABLE(chat_id integer, user_id integer, message_count bigint)
    LANGUAGE 'plpgsql'
    VOLATILE
    PARALLEL UNSAFE
    COST 100    ROWS 1000 
    
AS $BODY$
BEGIN
    RETURN QUERY(
        SELECT _chat_id
             , u.id as user_id
             , count(m.*) 
          FROM bot.messages m
          JOIN bot.users u ON u.id = m.user_id
         WHERE m.chat_id = _chat_id
           AND m.insert_date >= _from_date AND m.insert_date <= _to_date
           AND m.is_deleted = false
      GROUP BY u.id
      ORDER BY count(m.*) DESC
      LIMIT _users_limit);
END;
$BODY$;
ALTER FUNCTION bot.sf_get_chat_statistics(integer, integer, timestamp with time zone, timestamp with time zone)
    OWNER TO postgres;
COMMENT ON FUNCTION bot.sf_get_chat_statistics(integer, integer, timestamp with time zone, timestamp with time zone)
    IS 'Returns a list of users and the number of their messages in the specified time range';
--select * from bot.sf_get_chat_statistics(1, 10, now()::date - interval '1 day', now())




