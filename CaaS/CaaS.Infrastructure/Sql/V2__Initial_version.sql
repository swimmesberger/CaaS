DROP TABLE IF EXISTS shop;

-- add create table statements here

CREATE TABLE shop (
    id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    row_version int DEFAULT 0,
    creation_time timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP, -- this is only UTC in postgres
    last_modification_time timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    name varchar(255)
);
ALTER TABLE IF EXISTS public.shop OWNER to caas;
INSERT INTO public.shop(name) VALUES ('Amazon');
INSERT INTO public.shop(name) VALUES ('E-Tec');