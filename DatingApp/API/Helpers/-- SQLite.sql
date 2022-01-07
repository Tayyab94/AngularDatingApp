
SELECT * FROM Photos


DELETE from  photos where id =12

UPDATE Photos set Url= 'https://randomuser.me/api/portraits/women/91.jpg'
where Id =1


UPDATE Photos set Url= 'https://randomuser.me/api/portraits/women/88.jpg'
where Id =8
UPDATE Photos set IsMain =TRUE
where Id =8


UPDATE Photos set Url= 'https://randomuser.me/api/portraits/women/19.jpg'
where Id =9
UPDATE Photos set IsMain =TRUE
where Id =9


UPDATE Photos set Url= 'https://randomuser.me/api/portraits/women/73.jpg'
where Id =10

UPDATE Photos set IsMain =TRUE
where Id =10



UPDATE Photos set 
IsMain=false;
where Id =15