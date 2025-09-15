sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module3.Sample11.Exchange type=fanout durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module3.Sample11.NormalQueue durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module3.Sample11.Exchange destination=Module3.Sample11.NormalQueue

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module3.Sample11.HoldingQueue durable=true arguments='{"x-dead-letter-exchange":"Module3.Sample11.Exchange"}'