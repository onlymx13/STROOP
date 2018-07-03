﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace STROOP.Ttc
{
    /** An elevator is the yellow rectangle platform that moves up and down
     *  and will periodically stops and switches directions.
     *  
     *  An elevator moves up or down and will switch directions
     *  when it reaches its min height or max height. In addition,
     *  when the counter variable exceeds the max variable,
     *  the elevator will call RNG to determine whether its new
     *  direction (up or down) and how long until the next
     *  possible direction switch.
     */
    public class TtcElevator : TtcObject
    {

        public readonly int MIN_HEIGHT;
        public readonly int MAX_HEIGHT;

        public int _height;
        public int _verticalSpeed;
        public int _direction;
        public int _max;
        public int _counter;

        public TtcElevator(TtcRng rng, uint address) :
            this(rng, -100, -100, -100, 0, 1, 0, 0)
        {
        }

        public TtcElevator(TtcRng rng, int minHeight, int maxHeight) :
            this(rng, minHeight, maxHeight, minHeight, 0, 1, 0, 0)
        {
        }

        public TtcElevator(
            TtcRng rng, int minHeight, int maxHeight, int height,
            int verticalSpeed, int direction, int max, int counter) : base(rng)
        {
            MIN_HEIGHT = minHeight;
            MAX_HEIGHT = maxHeight;
            _height = height;
            _verticalSpeed = verticalSpeed;
            _direction = direction;
            _max = max;
            _counter = counter;
        }

        public override void Update()
        {
            if (_counter <= 4)
            {
                _verticalSpeed = 0;
            }
            else
            {
                _verticalSpeed = _direction * 6;
            }

            _height = _height + _verticalSpeed;

            if (_counter > _max)
            {
                _direction = (PollRNG() <= 32766) ? -1 : 1; // = -1, 1
                _max = (PollRNG() % 6) * 30 + 30; // = 30, 60, 90, 120, 150, 180
                _counter = 0;
            }

            _height = Math.Max(_height, MIN_HEIGHT);
            _height = Math.Min(_height, MAX_HEIGHT);
            if (_height == MIN_HEIGHT || _height == MAX_HEIGHT)
            {
                _direction *= -1;
            }
            _counter++;
        }

        public override string ToString()
        {
            return _id + OPENER + _height + SEPARATOR +
                      _verticalSpeed + SEPARATOR +
                      _direction + SEPARATOR +
                      _max + SEPARATOR +
                      _counter + CLOSER;
        }

    }


}