--
-- PostgreSQL database dump
--

-- Dumped from database version 14.0 (Debian 14.0-1.pgdg110+1)
-- Dumped by pg_dump version 14.0

-- Started on 2021-10-31 21:32:45

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
    "Id" uuid NOT NULL,
    "Action" text NOT NULL,
    "AggregateId" uuid NOT NULL,
    "Data" text NOT NULL,
    "TimeStamp" date NOT NULL,
    "Version" integer NOT NULL
);


ALTER TABLE public."LoggedEvent" OWNER TO postgres;

--
-- TOC entry 3167 (class 2606 OID 16391)
-- Name: LoggedEvent LoggedEvent_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LoggedEvent"
    ADD CONSTRAINT "LoggedEvent_pkey" PRIMARY KEY ("Id", "Version");


-- Completed on 2021-10-31 21:32:45

--
-- PostgreSQL database dump complete
--

