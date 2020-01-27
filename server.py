import socket
import threading
import random
import rsa
import os
import requests
import base64
#----Проверка клиента-------------------------
def testcli(id, conn):
	try:
		resp = requests.post('http://185.251.38.207/fromId.php?id=' + str(id)).text.split(';')[2]
		key = resp.split('.')
		mod = base64.b64decode(key[0].encode('UTF8'))
		exp = base64.b64decode(key[1].encode('UTF8'))
		intmod = 0
		for i in range(len(mod)):
			intmod *= 256
			intmod += int(mod[i])
		intexp = 0
		for i in range(len(exp)):
			intexp *= 256
			intexp += int(exp[i])
		pk = rsa.PublicKey(intmod, intexp)
		
		ranbyte = bytearray(16)
		for i in range(len(ranbyte)):
			ranbyte[i] = random.randint(0, 255)
		tosend = rsa.encrypt(ranbyte, pk)
		conn.send(tosend)
		if ranbyte != conn.recv(16):
			return False
		return True
	except:
		return False
#---------------------------------------------
#----Поток клиента----------------------------
def Client(conn, addr, soc):
	try:
		data = bytearray(conn.recv(260))

		saveid = 0
		for i in range(4):
			saveid = saveid + data[i] * 256**i
		
		if testcli(saveid, conn) == False:
			return
		print('Подключился:', addr, saveid)

		clients[saveid] = soc

		while 1:
			data = bytearray(conn.recv(260))
			To = 0
			for i in range(4):
				To = To + data[i] * 256**i
			for i in range(4):
				data[i] = int(saveid / 256 ** i) % 256
			SendTo(To, data)
	except Exception as ex:
		print('Произошла ошибка с клиентом - ', ex)
		conn.close()
		return
	conn.close()
#---------------------------------------------
#----Метод отправки сообщения-----------------
def SendTo(id, message):
	try:
		print('message send to: ', id)
		clients[id][0].send(message)
	except Exception as ex:
		print('Произошла ошибка с отправкой - ', ex)
#---------------------------------------------
#----Подключение нового клиента---------------
sock = socket.socket()
try:
	os.system('fuser -k 9090/tcp')
	sock.bind(('', 9090))
	sock.listen(0)

	clients = {}

	while 1:
		clientT = sock.accept()
		potok = threading.Thread(target=Client, args=(clientT[0], clientT[1], clientT))
		potok.start()
except Exception as ex:
	print('Произошла ошибка в добавлении клиента - ', ex)
	sock.close()
#---------------------------------------------