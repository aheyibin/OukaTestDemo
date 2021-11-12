@echo off

echo param[0] = %0
echo param[1] = %1

protogen --proto_path=..\Assets\Proto --csharp_out=..\Assets\Scripts\Net\protobuf-net %1

pause