INSERT INTO shop_admin(id,row_version,creation_time,last_modification_time,shop_id,name,e_mail)
VALUES ('bbfc1894-0c7e-4414-aa71-20d7cbbe7236',1,'2022-03-24 08:25:25','2022-09-07 01:50:55','a468d796-db09-496d-9794-f6b42f8c7c0b','Joye Killiam','jkilliam0@zdnet.com');
INSERT INTO shop_admin(id,row_version,creation_time,last_modification_time,shop_id,name,e_mail)
VALUES ('a5c992d5-5065-41fe-b2d0-2fa4b5945b56',5,'2022-05-31 18:29:50','2022-09-06 14:01:03','a468d796-db09-496d-9794-f6b42f8c7c0b','Kanya Pavey','kpavey0@google.ca');

INSERT INTO shop(id,row_version,creation_time,last_modification_time,name,cart_lifetime_minutes,admin_id,app_key)
VALUES ('a468d796-db09-496d-9794-f6b42f8c7c0b',1,'2022-06-29 21:22:51','2022-10-06 14:30:53','Amazon',44415,'bbfc1894-0c7e-4414-aa71-20d7cbbe7236','362a9325-ffb8-432b-bfd3-91c191fd5d69');
INSERT INTO shop(id,row_version,creation_time,last_modification_time,name,cart_lifetime_minutes,admin_id,app_key)
VALUES ('ba86a395-51ca-4f29-a047-5296ce90ab79',3,'2022-01-25 05:39:19','2022-09-30 15:40:10','E-Tec',32382,'bbfc1894-0c7e-4414-aa71-20d7cbbe7236','3ce7413c-ac69-459a-8d9f-b93a2e88a4f6');
INSERT INTO shop(id,row_version,creation_time,last_modification_time,name,cart_lifetime_minutes,admin_id,app_key)
VALUES ('c277a395-51ca-4f29-a047-5296ce90ab79',3,'2022-01-25 05:39:19','2022-09-30 15:40:10','Kanoodle',32382,'bbfc1894-0c7e-4414-aa71-20d7cbbe7236','7f272c83-97d5-4b95-a652-d15c4ba988bf');

INSERT INTO files (id, shop_id, name, path, mime_type, blob) VALUES ('581e4c62-9127-4b95-898b-2836ca3d90d2', 'a468d796-db09-496d-9794-f6b42f8c7c0b', 'veryday_clean_dandruff.pdf', 'downloads/581e4c62-9127-4b95-898b-2836ca3d90d2', 'application/pdf', '\x255044462d312e32200a392030206f626a0a3c3c0a3e3e0a73747265616d0a42542f203332205466282020594f55522054455854204845524520202029272045540a656e6473747265616d0a656e646f626a0a342030206f626a0a3c3c0a2f54797065202f506167650a2f506172656e742035203020520a2f436f6e74656e74732039203020520a3e3e0a656e646f626a0a352030206f626a0a3c3c0a2f4b696473205b3420302052205d0a2f436f756e7420310a2f54797065202f50616765730a2f4d65646961426f78205b2030203020323530203530205d0a3e3e0a656e646f626a0a332030206f626a0a3c3c0a2f50616765732035203020520a2f54797065202f436174616c6f670a3e3e0a656e646f626a0a747261696c65720a3c3c0a2f526f6f742033203020520a3e3e0a2525454f46');

INSERT INTO product(id,row_version,creation_time,last_modification_time,shop_id,name,description,download_link,price,deleted) 
    VALUES ('fcb3c98d-4392-4e4c-8d31-f89f0ebe3c83',8,'2022-05-31 10:38:18','2022-08-22 11:30:02','a468d796-db09-496d-9794-f6b42f8c7c0b','USB cable','liquam augue quam',
            'downloads/581e4c62-9127-4b95-898b-2836ca3d90d2',2.99,'false');
INSERT INTO product(id,row_version,creation_time,last_modification_time,shop_id,name,description,download_link,price,deleted) 
    VALUES ('ff66c1f5-d79e-4797-a03c-a665ae26b171',9,'2022-01-20 16:52:26','2022-08-26 16:35:52','a468d796-db09-496d-9794-f6b42f8c7c0b','HDMI cable',
            'tincidunt ante vel ipsum.','downloads/581e4c62-9127-4b95-898b-2836ca3d90d2',5.99,'false');
INSERT INTO product(id,row_version,creation_time,last_modification_time,shop_id,name,description,download_link,price,deleted) 
    VALUES ('587c3437-b430-405a-99dd-a0ce9ebde0a4',1,'2022-03-17 00:49:17','2022-08-25 06:39:49','a468d796-db09-496d-9794-f6b42f8c7c0b','LAN cable',
            'estibulum ante','downloads/581e4c62-9127-4b95-898b-2836ca3d90d2',7.96,'false');

INSERT INTO customer(id,row_version,creation_time,last_modification_time,shop_id,name,e_mail,credit_card_number) 
    VALUES ('9234a988-0abd-4b44-808a-9e7a8852e19c',2,'2022-01-09 19:04:52','2022-10-21 05:29:31','a468d796-db09-496d-9794-f6b42f8c7c0b',
            'Frances Hallums','fhallums2r@edublogs.org','3581814560128824');
INSERT INTO customer(id,row_version,creation_time,last_modification_time,shop_id,name,e_mail,credit_card_number) 
    VALUES ('703111b0-c3fd-4bf1-9d1a-12cd3852c182',2,'2022-01-15 15:06:22','2022-10-08 22:52:24','a468d796-db09-496d-9794-f6b42f8c7c0b',
            'Lebbie Mancell','lmancell1@timesonline.co.uk','4844714712836418');
INSERT INTO customer(id,row_version,creation_time,last_modification_time,shop_id,name,e_mail,credit_card_number) 
    VALUES ('c63b840a-a520-4a6a-a5d1-7328618c20c5',1,'2021-12-23 13:09:31','2022-10-26 13:50:23','a468d796-db09-496d-9794-f6b42f8c7c0b',
            'Alex Cossentine','acossentine9@lycos.com','5018119876168150');

INSERT INTO "order"(id,row_version,creation_time,last_modification_time,shop_id,order_number,customer_id,order_date) 
    VALUES ('41b1fa55-fd34-4611-bda0-f3821f6db52b',4,'2022-07-18 09:38:13','2022-09-22 03:26:33','a468d796-db09-496d-9794-f6b42f8c7c0b',
            2628,'9234a988-0abd-4b44-808a-9e7a8852e19c','2022-07-18 09:38:13');
INSERT INTO "order"(id,row_version,creation_time,last_modification_time,shop_id,order_number,customer_id,order_date)
    VALUES ('aca58853-bd61-4517-9907-ca51a50b7225',4,'2022-07-18 09:38:13','2022-09-22 03:26:33','a468d796-db09-496d-9794-f6b42f8c7c0b',
            7785,'9234a988-0abd-4b44-808a-9e7a8852e19c','2022-07-08 09:38:13');

INSERT INTO product_order(id,row_version,creation_time,last_modification_time,shop_id,product_id,order_id,amount,price_per_piece) 
    VALUES ('363bffa6-e73d-4aa1-be30-aa636fa823c0',9,'2022-01-04 08:00:50','2022-05-10 08:01:26','a468d796-db09-496d-9794-f6b42f8c7c0b',
            'fcb3c98d-4392-4e4c-8d31-f89f0ebe3c83','41b1fa55-fd34-4611-bda0-f3821f6db52b',4,13.79);
INSERT INTO product_order(id,row_version,creation_time,last_modification_time,shop_id,product_id,order_id,amount,price_per_piece) 
    VALUES ('1f3c4771-7ec1-4017-a531-10dea11c0745',10,'2022-01-17 19:58:18','2022-04-22 19:58:51','a468d796-db09-496d-9794-f6b42f8c7c0b',
            'ff66c1f5-d79e-4797-a03c-a665ae26b171','41b1fa55-fd34-4611-bda0-f3821f6db52b',4,14.55);

INSERT INTO cart(id,row_version,creation_time,last_modification_time,shop_id,customer_id,last_access) 
    VALUES ('b4cf2977-13fc-44dd-89cf-0dbfdae04fce',22,'2021-12-24 19:46:11','2022-08-14 04:57:22','a468d796-db09-496d-9794-f6b42f8c7c0b',
            '703111b0-c3fd-4bf1-9d1a-12cd3852c182','2022-08-13 07:01:46');
INSERT INTO cart(id,row_version,creation_time,last_modification_time,shop_id,customer_id,last_access) 
    VALUES ('4c99b9b8-9cb5-4a49-ada9-01bd95f398d4',41,'2021-12-13 23:28:01','2022-04-22 00:54:22','a468d796-db09-496d-9794-f6b42f8c7c0b',
            NULL,'2022-08-16 22:58:16');

INSERT INTO product_cart(id,row_version,creation_time,last_modification_time,shop_id,product_id,cart_id,amount) 
    VALUES ('f4ef7180-016b-400a-a24e-797b7feb7780',2,'2022-03-13 00:31:27','2022-09-25 01:03:08','a468d796-db09-496d-9794-f6b42f8c7c0b',
            'fcb3c98d-4392-4e4c-8d31-f89f0ebe3c83','b4cf2977-13fc-44dd-89cf-0dbfdae04fce',4);
INSERT INTO product_cart(id,row_version,creation_time,last_modification_time,shop_id,product_id,cart_id,amount) 
    VALUES ('77894ac7-e360-44b2-a873-2710b3114fdf',5,'2022-07-07 03:28:09','2022-10-05 17:00:06','a468d796-db09-496d-9794-f6b42f8c7c0b',
            'ff66c1f5-d79e-4797-a03c-a665ae26b171','b4cf2977-13fc-44dd-89cf-0dbfdae04fce',4);

INSERT INTO coupon(id,row_version,creation_time,last_modification_time,shop_id,code,value,order_id,cart_id,customer_id) 
    VALUES ('bbf7b266-2485-4cd8-a0a4-822a692fab10',4,'2021-11-15 18:20:52','2022-09-14 14:12:15','a468d796-db09-496d-9794-f6b42f8c7c0b','SUMMER1234',10,
            '41b1fa55-fd34-4611-bda0-f3821f6db52b',NULL,'703111b0-c3fd-4bf1-9d1a-12cd3852c182');
INSERT INTO coupon(id,row_version,creation_time,last_modification_time,shop_id,code,value,order_id,cart_id,customer_id) 
    VALUES ('aff66783-ed9c-4838-9642-72042883fffe',3,'2022-04-30 07:25:07','2022-09-13 10:43:50','a468d796-db09-496d-9794-f6b42f8c7c0b','SUMMER1235',7,
            NULL,'b4cf2977-13fc-44dd-89cf-0dbfdae04fce','703111b0-c3fd-4bf1-9d1a-12cd3852c182');

INSERT INTO discount_setting(id,row_version,creation_time,last_modification_time,shop_id,name,rule_id,action_id,rule_parameters,action_parameters) 
    VALUES ('7981f1dc-76d4-4d7b-b823-af2e841d8001',0,'2022-06-14 17:04:16','2022-10-24 18:28:33','a468d796-db09-496d-9794-f6b42f8c7c0b','Christmas2022',
            'B5791E0F-D839-45CE-92FE-94F3F5F19DEF','68A4020D-A8AC-4A74-8A04-24E449786898','{}','{}');
INSERT INTO discount_setting(id,row_version,creation_time,last_modification_time,shop_id,name,rule_id,action_id,rule_parameters,action_parameters) 
    VALUES ('b04bf20d-0abf-47c4-94d9-185536df9867',8,'2022-08-26 02:11:16','2022-09-10 19:13:43','a468d796-db09-496d-9794-f6b42f8c7c0b','Black Friday',
            'B5791E0F-D839-45CE-92FE-94F3F5F19DEF','68A4020D-A8AC-4A74-8A04-24E449786898','{}','{}');
INSERT INTO discount_setting(id,row_version,creation_time,last_modification_time,shop_id,name,rule_id,action_id,rule_parameters,action_parameters) 
    VALUES ('2f87ca9b-9829-437e-b78b-f71dcc2de7a0',7,'2021-12-02 11:31:37','2022-09-08 00:06:21','a468d796-db09-496d-9794-f6b42f8c7c0b','Valentines day',
            'B5791E0F-D839-45CE-92FE-94F3F5F19DEF','68A4020D-A8AC-4A74-8A04-24E449786898','{}','{}');

INSERT INTO order_discount(id,row_version,creation_time,last_modification_time,discount_name,discount,order_id,shop_id) 
    VALUES ('ad6f89c5-68dc-499d-ae9e-0976cad053fd',81,'2022-08-04 17:41:39','2022-09-30 21:26:54','Christmas2022',5,'41b1fa55-fd34-4611-bda0-f3821f6db52b',
            'a468d796-db09-496d-9794-f6b42f8c7c0b');
INSERT INTO order_discount(id,row_version,creation_time,last_modification_time,discount_name,discount,order_id,shop_id) 
    VALUES ('94c8b5ab-df5d-4aee-9391-d0189ef03fe4',65,'2021-11-17 01:52:38','2022-09-09 12:47:40','Black Friday',5,'41b1fa55-fd34-4611-bda0-f3821f6db52b',
            'a468d796-db09-496d-9794-f6b42f8c7c0b');

INSERT INTO product_order_discount(id,row_version,creation_time,last_modification_time,discount_name,discount,product_order_id,shop_id) 
    VALUES ('b7ab7819-6f94-467c-ac2e-79d857664ee7','6','2022-07-24 15:37:35','2022-09-29 08:38:22','-10% on everything','1.62','363bffa6-e73d-4aa1-be30-aa636fa823c0',
            'a468d796-db09-496d-9794-f6b42f8c7c0b');
INSERT INTO product_order_discount(id,row_version,creation_time,last_modification_time,discount_name,discount,product_order_id,shop_id) 
    VALUES ('8ac6ba97-921f-45f6-a8dd-a89d688cce83','9','2022-05-20 23:20:31','2022-09-21 03:28:10','most expensive product for free','1.59','363bffa6-e73d-4aa1-be30-aa636fa823c0',
            'a468d796-db09-496d-9794-f6b42f8c7c0b');
