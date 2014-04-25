@echo OFF
REM ################################################################################
REM Author     : Sylvain Desrochers
REM Description: Builds bijou in the windows world
REM ################################################################################
if "%1"=="" goto usage
if "%1"=="-bin" goto bin
if "%1"=="-dbg" goto dbg
if "%1"=="-unx" goto unx

REM ################################################################################
:bin
REM ################################################################################
pushd src
csc /out:../bin/Bijou.exe *.cs
popd 
goto exit

REM ################################################################################
:dbg
REM ################################################################################
pushd src
csc /debug /out:../bin/Bijou.exe *.cs
popd 
copy bin\bijou.exe ..\..\sly.ware\unix\bin
goto exit

REM ################################################################################
:unx
REM ################################################################################
copy bin\bijou.exe ..\..\slyware\unix\bin
goto exit

REM ################################################################################
:usage
REM ################################################################################
echo usage: mk [option]
echo           -bin recompiles the project
echo           -dbg recompiles in debug mode (with a beakpoint)
echo           -unx copies the binaries to slyware folder
goto exit
:exit
