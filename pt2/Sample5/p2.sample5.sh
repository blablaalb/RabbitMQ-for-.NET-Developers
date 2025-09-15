sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module3.Sample9.DeadLetterExchange type=fanout durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module3.Sample9.DeadLetter durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module3.Sample9.DeadLetterExchange destination=Module3.Sample9.DeadLetter
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module3.Sample9.Normal durable=true arguments='{"x-dead-letter-exchange":"Module3.Sample9.DeadLetterExchange"}'