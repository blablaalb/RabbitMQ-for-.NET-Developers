sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module2.Sample6.Exchange type=headers durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample6.Queue1 durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample6.Queue2 durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample6.Queue3 durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module2.Sample6.Queue4 durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample6.Exchange destination=Module2.Sample6.Queue1 arguments='{"material":"wood", "customertype":"b2b"}'
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample6.Exchange destination=Module2.Sample6.Queue2 arguments='{"material":"metal", "customertype":"b2c"}'
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample6.Exchange destination=Module2.Sample6.Queue3 arguments='{"x-match":"any", "material":"wood", "customertype":"b2b"}'
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module2.Sample6.Exchange destination=Module2.Sample6.Queue4 arguments='{"x-match":"any", "material":"metal", "customertype":"b2c"}'
