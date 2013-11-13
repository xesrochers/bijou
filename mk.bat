@echo OFF
REM ################################################################################
REM Author     : Sylvain Desrochers
REM Description: Builds bijou in the windows world
REM ################################################################################
if "%1"=="" goto usage
if "%1"=="-bin" goto bin
if "%1"=="-unx" goto unx

REM ################################################################################
:bin
REM ################################################################################
pushd src
csc /out:../bin/Bijou.exe *.cs
popd 
goto exit

REM ################################################################################
:unx
REM ################################################################################
copy bin\bijou.exe ..\..\sly.ware\unix\bin
goto exit

REM ################################################################################
:usage
REM ################################################################################
echo usage: mk [option]
echo           -bin recompiles the project
echo           -unx copies the binaries to slyware folder
goto exit
:exit
