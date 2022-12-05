ALTER TABLE IF EXISTS shop
    DROP CONSTRAINT "FK_shop.admin_id";

ALTER TABLE IF EXISTS product
    DROP CONSTRAINT "FK_product.shop_id";

ALTER TABLE IF EXISTS customer
    DROP CONSTRAINT "FK_customer.shop_id";

ALTER TABLE IF EXISTS "order"
    DROP CONSTRAINT "FK_order.shop_id";

ALTER TABLE IF EXISTS "order"
    DROP CONSTRAINT "FK_order.customer_id";

ALTER TABLE IF EXISTS product_order
    DROP CONSTRAINT "FK_product_order.product_id";

ALTER TABLE IF EXISTS product_order
    DROP CONSTRAINT "FK_product_order.order_id";

ALTER TABLE IF EXISTS product_order
    DROP CONSTRAINT "FK_product_order.shop_id";

ALTER TABLE IF EXISTS cart
    DROP CONSTRAINT "FK_cart.customer_id";

ALTER TABLE IF EXISTS cart
    DROP CONSTRAINT "FK_cart.shop_id";

ALTER TABLE IF EXISTS product_cart
    DROP CONSTRAINT "FK_product_cart.product_id";

ALTER TABLE IF EXISTS product_cart
    DROP CONSTRAINT "FK_product_cart.cart_id";

ALTER TABLE IF EXISTS product_cart
    DROP CONSTRAINT "FK_product_cart.shop_id";

ALTER TABLE IF EXISTS discount_setting
    DROP CONSTRAINT "FK_discount_setting.shop_id";

ALTER TABLE IF EXISTS coupon
    DROP CONSTRAINT "FK_coupon.customer_id";

ALTER TABLE IF EXISTS coupon
    DROP CONSTRAINT "FK_coupon.cart_id";

ALTER TABLE IF EXISTS coupon
    DROP CONSTRAINT "FK_coupon.order_id";

ALTER TABLE IF EXISTS product_order_discount
    DROP CONSTRAINT "FK_product_order_discount.product_order_id";

ALTER TABLE IF EXISTS product_order_discount
    DROP CONSTRAINT "FK_product_order_discount.shop_id";

ALTER TABLE IF EXISTS order_discount
    DROP CONSTRAINT "FK_order_discount.order_id";

ALTER TABLE IF EXISTS order_discount
    DROP CONSTRAINT "FK_order_discount.shop_id";

DROP TABLE IF EXISTS shop;
DROP TABLE IF EXISTS shop_admin;
DROP TABLE IF EXISTS product;
DROP TABLE IF EXISTS "order";
DROP TABLE IF EXISTS product_order;
DROP TABLE IF EXISTS customer;
DROP TABLE IF EXISTS cart;
DROP TABLE IF EXISTS product_cart;
DROP TABLE IF EXISTS discount_setting;
DROP TABLE IF EXISTS coupon;
DROP TABLE IF EXISTS product_order_discount;
DROP TABLE IF EXISTS order_discount;

-- add create table statements here

CREATE TABLE "shop_admin" (
    "id" uuid DEFAULT  gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP, -- this is only UTC in postgres
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "shop_id" uuid NOT NULL,
    "name" varchar(255) NOT NULL,
    "e_mail" varchar(255) UNIQUE NOT NULL -- should be email
);

CREATE TABLE shop (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "name" varchar(255) NOT NULL,
    "cart_lifetime_minutes" int default 120 NOT NULL,
    "admin_id" uuid,
    CONSTRAINT "FK_shop.admin_id"
        FOREIGN KEY ("admin_id")
            REFERENCES "shop_admin"("id") ON DELETE SET NULL
);

CREATE TABLE "product" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "shop_id" uuid NOT NULL,
    "name" varchar(255) NOT NULL,
    "description" varchar(255),
    "download_link" varchar(255) NOT NULL,
    "price" decimal NOT NULL,
    "deleted" boolean NOT NULL,
    CONSTRAINT "FK_product.shop_id"
       FOREIGN KEY ("shop_id")
        REFERENCES "shop"("id") ON DELETE CASCADE
);

CREATE TABLE "customer" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "shop_id" uuid NOT NULL,
    "name" varchar(255) NOT NULL,
    "e_mail" varchar(255) UNIQUE NOT NULL,
    "credit_card_number" varchar(19),
    CONSTRAINT "FK_customer.shop_id"
        FOREIGN KEY ("shop_id")
            REFERENCES "shop"("id") ON DELETE CASCADE
); 

CREATE TABLE "order" (
     "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
     "row_version" int DEFAULT 0,
     "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
     "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
     "shop_id" uuid NOT NULL,
     "order_number" SERIAL,
     "customer_id" uuid,
     "address_street" varchar(255),
     "address_city" varchar(255),
     "address_state" varchar(255),
     "address_country" varchar(255),
     "address_zip_code" varchar(255),
     "order_date" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
     CONSTRAINT "FK_order.shop_id"
         FOREIGN KEY ("shop_id")
             REFERENCES "shop"("id") ON DELETE CASCADE,
    CONSTRAINT "FK_order.customer_id"
         FOREIGN KEY ("customer_id")
             REFERENCES "customer"("id") ON DELETE SET NULL
);

CREATE TABLE "product_order" (
     "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
     "row_version" int DEFAULT 0,
     "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
     "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
     "shop_id" uuid NOT NULL,
     "product_id" uuid NOT NULL,
     "order_id" uuid NOT NULL,
     "amount" int NOT NULL,
     "price_per_piece" decimal NOT NULL,
     CONSTRAINT  "UNIQUE_product_id_order_id" UNIQUE (product_id, order_id),
     CONSTRAINT "FK_product_order.product_id"
         FOREIGN KEY ("product_id")
             REFERENCES "product"("id") ON DELETE NO ACTION,
     CONSTRAINT "FK_product_order.order_id"
         FOREIGN KEY ("order_id")
             REFERENCES "order"("id") ON DELETE CASCADE,
     CONSTRAINT "FK_product_order.shop_id"
         FOREIGN KEY ("shop_id")
             REFERENCES "shop"("id") ON DELETE CASCADE 
);

CREATE TABLE "cart" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "shop_id" uuid NOT NULL,
    "customer_id" uuid,
    "last_access" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT "FK_cart.customer_id"
        FOREIGN KEY ("customer_id")
            REFERENCES "customer"("id") ON DELETE CASCADE,
    CONSTRAINT "FK_cart.shop_id"
        FOREIGN KEY ("shop_id")
            REFERENCES "shop"("id") ON DELETE CASCADE
);

CREATE TABLE "product_cart" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0 NOT NULL,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "shop_id" uuid NOT NULL,
    "product_id" uuid NOT NULL,
    "cart_id" uuid NOT NULL,
    "amount" int NOT NULL,
    CONSTRAINT  "UNIQUE_product_id_cart_id" UNIQUE (product_id, cart_id),
    CONSTRAINT "FK_product_cart.product_id"
        FOREIGN KEY ("product_id")
            REFERENCES "product"("id") ON DELETE NO ACTION,
    CONSTRAINT "FK_product_cart.cart_id"
        FOREIGN KEY ("cart_id")
            REFERENCES "cart"("id") ON DELETE CASCADE,
    CONSTRAINT "FK_product_cart.shop_id"
        FOREIGN KEY ("shop_id")
            REFERENCES "shop"("id") ON DELETE CASCADE
);

CREATE TABLE "discount_setting" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0 NOT NULL,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "shop_id" uuid NOT NULL,
    "name" varchar(255),
    "rule_id" uuid NOT NULL,
    "action_id" uuid NOT NULL,
    "rule_parameters" jsonb,
    "action_parameters" jsonb,
    CONSTRAINT "FK_discount_setting.shop_id"
        FOREIGN KEY ("shop_id")
            REFERENCES "shop"("id") ON DELETE CASCADE
);

CREATE TABLE "coupon" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0 NOT NULL,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "shop_id" uuid NOT NULL,
    "value" decimal NOT NULL,
    "order_id" uuid,
    "cart_id" uuid,
    "customer_id" uuid,
    CONSTRAINT "FK_coupon.customer_id"
      FOREIGN KEY ("customer_id")
          REFERENCES "customer"("id") ON DELETE SET NULL,
    CONSTRAINT "FK_coupon.cart_id"
      FOREIGN KEY ("cart_id")
          REFERENCES "cart"("id") ON DELETE SET NULL,
    CONSTRAINT "FK_coupon.order_id"
      FOREIGN KEY ("order_id")
          REFERENCES "order"("id") ON DELETE SET NULL
);

CREATE TABLE "product_order_discount" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0 NOT NULL,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "discount_name" varchar(255) NOT NULL ,
    "discount" decimal NOT NULL ,
    "product_order_id" uuid NOT NULL,
    "shop_id" uuid NOT NULL,
    CONSTRAINT "FK_product_order_discount.product_order_id"
      FOREIGN KEY ("product_order_id")
          REFERENCES "product_order"("id") ON DELETE CASCADE,
    CONSTRAINT "FK_product_order_discount.shop_id"
      FOREIGN KEY ("shop_id")
          REFERENCES "shop"("id") ON DELETE CASCADE 
);

CREATE TABLE "order_discount" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "row_version" int DEFAULT 0 NOT NULL,
    "creation_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "last_modification_time" timestamp WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "discount_name" varchar(255) NOT NULL,
    "discount" decimal NOT NULL,
    "order_id" uuid NOT NULL,
    "shop_id" uuid NOT NULL,
    CONSTRAINT "FK_order_discount.order_id"
      FOREIGN KEY ("order_id")
          REFERENCES "order"("id") ON DELETE CASCADE,
    CONSTRAINT "FK_order_discount.shop_id"
      FOREIGN KEY ("shop_id")
          REFERENCES "shop"("id") ON DELETE CASCADE
);

ALTER TABLE IF EXISTS public.shop OWNER to caas;
ALTER TABLE IF EXISTS public.shop_admin OWNER to caas;
ALTER TABLE IF EXISTS public.product OWNER to caas;
ALTER TABLE IF EXISTS public.order OWNER to caas;
ALTER TABLE IF EXISTS public.product_order OWNER to caas;
ALTER TABLE IF EXISTS public.customer OWNER to caas;
ALTER TABLE IF EXISTS public.cart OWNER to caas;
ALTER TABLE IF EXISTS public.product_cart OWNER to caas;
ALTER TABLE IF EXISTS public.discount_setting OWNER to caas;
ALTER TABLE IF EXISTS public.coupon OWNER to caas;
ALTER TABLE IF EXISTS public.product_order_discount OWNER to caas;
ALTER TABLE IF EXISTS public.order_discount OWNER to caas;
