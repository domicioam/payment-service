--
-- PostgreSQL database dump
--

-- Dumped from database version 14.0 (Debian 14.0-1.pgdg110+1)
-- Dumped by pg_dump version 14.0

-- Started on 2021-10-31 21:39:41

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
-- Name: Merchant; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Merchant" (
    "Id" uuid NOT NULL,
    "IsActive" bit(1) NOT NULL
);


ALTER TABLE public."Merchant" OWNER TO postgres;

--
-- TOC entry 3307 (class 0 OID 16385)
-- Dependencies: 209
-- Data for Name: Merchant; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Merchant" ("Id", "IsActive") FROM stdin;
3fa85f64-5717-4562-b3fc-2c963f66afa6	1
\.


--
-- TOC entry 3167 (class 2606 OID 16389)
-- Name: Merchant Merchant_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Merchant"
    ADD CONSTRAINT "Merchant_pkey" PRIMARY KEY ("Id");


-- Completed on 2021-10-31 21:39:42

--
-- PostgreSQL database dump complete
--

