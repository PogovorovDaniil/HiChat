<?php 
$host = 'localhost'; // адрес сервера 
$database = '********'; // имя базы данных
$user = 'root'; // имя пользователя
$password = '********'; // пароль

$link = mysqli_connect($host, $user, $password, $database) 
or die("0");
$query = 'INSERT INTO users (name, pubkey) VALUES ("' . addslashes($_POST['name']) . '", "' . addslashes($_POST['pubkey']) . '")';

$result = mysqli_query($link, $query) or die("0"); 
if($result) echo('1');
mysqli_close($link);
?>