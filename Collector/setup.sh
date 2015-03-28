busid=1
datasrcip=$1
instance=$2
pmudumperip=$3
startport=$4

#./multi_pmudumper -c $startport -i $instance -s 5300 &
echo $busid $instance
while [ $busid -le $instance ]
do
  let port=$startport+$busid-1
  echo "dc $datasrcip 15000 $busid $pmudumperip $port"
  ./dc $datasrcip 15000 $busid $pmudumperip $port &>"dc_$busid.txt" &
  let busid=$busid+1
done
