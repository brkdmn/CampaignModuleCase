

CREATE TABLE IF NOT EXISTS public."product"
(
    "id" uuid NOT NULL,
    "product_code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "price" numeric(18,2) NOT NULL,
    "stock" integer NOT NULL,
    "is_active" boolean NOT NULL,
    "is_deleted" boolean NOT NULL,
    "created_date" timestamp without time zone NOT NULL,
    "updated_date" timestamp without time zone,
    
    CONSTRAINT "PK_Product" PRIMARY KEY ("id")
);


CREATE TABLE IF NOT EXISTS public."order"
(
    "id" uuid NOT NULL,
    "product_code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "campaign_name" character varying(128) COLLATE pg_catalog."default" NOT NULL,
    "quantity" integer NOT NULL,
    "sale_price" numeric(18,2) NOT NULL,
    "is_active" boolean NOT NULL,
    "is_deleted" boolean NOT NULL,
    "created_date" timestamp without time zone NOT NULL,
    "updated_date" timestamp without time zone,
    CONSTRAINT "PK_Order" PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS public."campaign"
(
    "id" uuid NOT NULL,
    "name" character varying(128) COLLATE pg_catalog."default" NOT NULL,
    "product_code" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "duration" integer NOT NULL,
    "current_duration" integer NOT NULL,
    "price_manipulation_limit" integer NOT NULL,
    "target_sales_count" integer NOT NULL,
    "is_active" boolean NOT NULL,
    "is_deleted" boolean NOT NULL,
    "created_date" timestamp without time zone NOT NULL,
    "updated_date" timestamp without time zone,
    
    CONSTRAINT "PK_Campaign" PRIMARY KEY ("id")
);
