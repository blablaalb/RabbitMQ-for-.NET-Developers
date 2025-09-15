sudo rabbitmqctl add_vhost dot-net-course
sudo rabbitmqctl set_permissions -p dot-net-course admin ".*" ".*" ".*"
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module3.Sample10.FailuresExchange type=fanout durable=true

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module3.Sample10.Failures durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module3.Sample10.FailuresExchange destination=Module3.Sample10.Failures

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare exchange name=Module3.Sample10.Exchange type=topic durable=true arguments='{"alternate-exchange":"Module3.Sample10.FailuresExchange"}'

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module3.Sample10.Apples durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module3.Sample10.Exchange destination=Module3.Sample10.Apples routing_key="apples"

sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare queue name=Module3.Sample10.Oranges durable=true
sudo rabbitmqadmin -V dot-net-course --username=admin --password=password declare binding source=Module3.Sample10.Exchange destination=Module3.Sample10.Oranges routing_key="oranges"