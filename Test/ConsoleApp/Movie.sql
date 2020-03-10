-- Create oracle table
create table MOVIE
(
  id          NUMBER(19) default 0 not null,
  title       VARCHAR2(60) default ' ' not null,
  releasedate VARCHAR2(20) default ' ' not null,
  genre       VARCHAR2(60) default ' ' not null,
  price       NUMBER(29,9) default 0 not null
)
tablespace USERS
  pctfree 10
  initrans 1
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  );
-- Create/Recreate primary, unique and foreign key constraints 
alter table MOVIE
  add constraint PK_MOVIE primary key (ID)
  using index 
  tablespace USERS
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  );
