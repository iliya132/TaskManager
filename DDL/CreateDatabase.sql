use TaskManager;
drop table if exists TasksToAnalytics;
drop table if exists TaskPlan;
drop table if exists Tasks;
drop table if exists Statuses;


create table Statuses(
	id int primary key identity (1,1),
	name nvarchar(50)
);

create table Tasks(
	id int primary key identity (1,1),
	subject nvarchar(max) not null,
	created_at datetime not null default GetDate(),
	updated_at datetime not null default GetDate(),
	due_date datetime not null,
	created_by int not null,
	awaited_result nvarchar(max),
	description nvarchar(max),
	status int not null default 1,
	parent_task int,
	foreign key (status) references Statuses(id),
	foreign key (parent_task) references Tasks(id)
);

select * from Tasks;

create table TasksToAnalytics(
	id int primary key identity(1,1),
	task int not null,
	analytic int not null,
	comment nvarchar(max),
	status int not null default 1,
	updated_at datetime not null default GetDate(),
	foreign key (task) references Tasks(id),
	foreign key(status) references Statuses(id)
);

go
create trigger Tasks_update
on Tasks
after update
as
update Tasks set updated_At = GETDATE() 
where id in (select id from inserted);
go

create trigger TasksToAnalytics_update
on TasksToAnalytics
after update
as
update TasksToAnalytics set updated_At = GETDATE() 
where id in (select id from inserted);
go

create table TaskPlan(
	id int primary key identity (1,1),
	task int not null,
	description nvarchar(max) not null,
	created_at datetime not null default GetDate(),
	created_by int not null,
	due_date date not null,
	foreign key (task) references Tasks(id)
);

