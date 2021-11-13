--
-- PostgreSQL database dump
--

-- Dumped from database version 14.0 (Debian 14.0-1.pgdg110+1)
-- Dumped by pg_dump version 14.0

-- Started on 2021-11-13 13:02:08

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 209 (class 1259 OID 16385)
-- Name: LoggedEvent; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."LoggedEvent" (
    "Action" text NOT NULL,
    "AggregateId" uuid NOT NULL,
    "Data" text NOT NULL,
    "Version" integer NOT NULL,
    "TimeStamp" timestamp with time zone NOT NULL,
    "Id" integer NOT NULL
);


ALTER TABLE public."LoggedEvent" OWNER TO postgres;

--
-- TOC entry 210 (class 1259 OID 16392)
-- Name: LoggedEvent_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."LoggedEvent_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."LoggedEvent_Id_seq" OWNER TO postgres;

--
-- TOC entry 3315 (class 0 OID 0)
-- Dependencies: 210
-- Name: LoggedEvent_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."LoggedEvent_Id_seq" OWNED BY public."LoggedEvent"."Id";


--
-- TOC entry 3167 (class 2604 OID 16393)
-- Name: LoggedEvent Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LoggedEvent" ALTER COLUMN "Id" SET DEFAULT nextval('public."LoggedEvent_Id_seq"'::regclass);


--
-- TOC entry 3169 (class 2606 OID 16400)
-- Name: LoggedEvent LoggedEvent_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LoggedEvent"
    ADD CONSTRAINT "LoggedEvent_pkey" PRIMARY KEY ("Id");


-- Completed on 2021-11-13 13:02:08

--
-- PostgreSQL database dump complete
--

