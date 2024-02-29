"""
To test game inputs, run this program in a terminal and then try to
decode the input from reading a terminal.
"""

import time
import argparse
import subprocess
from fir_tester import FPGA, Controller

NIOS_TERMINAL = "nios2-terminal.exe"


def simulate():
    # The sampling rate of the FPGA accelerometer
    SAMPLING_RATE = 1000

    fpga = FPGA()
    controller = Controller()

    while True:
        output = fpga.output(
            controller.make_random_movement(),
            0,
            controller.get_key(),
            controller.get_key()
        )
        print(FPGA.uart_decode(output))
        time.sleep(1.0 / SAMPLING_RATE)


def real():
    process = subprocess.Popen([NIOS_TERMINAL],
                               stdout=subprocess.PIPE,
                               stderr=subprocess.DEVNULL)

    # Skip first 100 lines (e.g. "nios2-terminal: connected to hardware target")
    for _ in range(100):
        process.stdout.readline()

    while True:
        line = process.stdout.readline().decode('utf-8').strip()
        print(FPGA.uart_decode(line))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "-r", "--real",
        action="store_true",
        help="Run the program with real inputs"
    )
    args = parser.parse_args()

    if args.real:
        real()
    else:
        simulate()


if __name__ == '__main__':
    main()
