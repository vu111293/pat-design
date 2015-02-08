//////////////////////////////////////////////////////////////////////////////
// Product:  QF/C++ port to Win32
// Last Updated for Version: 4.3.00
// Date of the Last Update:  Nov 02, 2011
//
//                    Q u a n t u m     L e a P s
//                    ---------------------------
//                    innovating embedded systems
//
// Copyright (C) 2002-2011 Quantum Leaps, LLC. All rights reserved.
//
// This software may be distributed and modified under the terms of the GNU
// General Public License version 2 (GPL) as published by the Free Software
// Foundation and appearing in the file GPL.TXT included in the packaging of
// this file. Please note that GPL Section 2[b] requires that all works based
// on this software must also be made publicly available under the terms of
// the GPL ("Copyleft").
//
// Alternatively, this software may be distributed and modified under the
// terms of Quantum Leaps commercial licenses, which expressly supersede
// the GPL and are specifically designed for licensees interested in
// retaining the proprietary status of their code.
//
// Contact information:
// Quantum Leaps Web site:  http://www.quantum-leaps.com
// e-mail:                  info@quantum-leaps.com
//////////////////////////////////////////////////////////////////////////////
#include "qf_pkg.h"
#include "qassert.h"

#include <stdio.h>

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

Q_DEFINE_THIS_MODULE(qf_port)

// Global objects ------------------------------------------------------------
CRITICAL_SECTION QF_win32CritSect_;

// Local objects -------------------------------------------------------------
static DWORD WINAPI thread_function(LPVOID arg);
static DWORD l_tickMsec = 10;     // clock tick in msec (argument for Sleep())
static uint8_t l_running;

#ifdef Q_SPY
static uint8_t l_ticker;
#endif

//............................................................................
const char Q_ROM *QF::getPortVersion(void) {
    static const char Q_ROM version[] =  "4.3.00";
    return version;
}
//............................................................................
void QF::init(void) {
    InitializeCriticalSection(&QF_win32CritSect_);
}
//............................................................................
void QF::stop(void) {
    l_running = (uint8_t)0;
}
//............................................................................
void QF::run(void) {
    l_running = (uint8_t)1;
    onStartup();                                           // startup callback

               // raise the priority of this (main) thread to tick more timely
    SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_TIME_CRITICAL);

    QS_OBJ_DICTIONARY(&l_ticker);          // the QS dictionary for the ticker

    while (l_running) {
        TICK(&l_ticker);                                // process a time tick
        Sleep(l_tickMsec);                       // wait for the tick interval
    }
    onCleanup();                                           // cleanup callback
    QS_EXIT();                                  // cleanup the QSPY connection
    DeleteCriticalSection(&QF_win32CritSect_);
}
//............................................................................
void QF_setTickRate(uint32_t ticksPerSec) {
    l_tickMsec = 1000UL / ticksPerSec;
}
//............................................................................
void QActive::start(uint8_t prio,
                    QEvent const *qSto[], uint32_t qLen,
                    void *stkSto, uint32_t stkSize,
                    QEvent const *ie)
{
    Q_REQUIRE((stkSto == (void *)0)  /* Windows allocates stack internally */
              && (stkSize != 0));

    m_eQueue.init(qSto, (QEQueueCtr)qLen);
    m_osObject = CreateEvent(NULL, FALSE, FALSE, NULL);
    m_prio = prio;
    QF::add_(this);                     // make QF aware of this active object
    init(ie);                                    // execute initial transition

    m_thread = CreateThread(NULL, stkSize, &thread_function, this, 0, NULL);
    Q_ASSERT(m_thread != (HANDLE)0);                 // thread must be created

    int p;
    switch (m_prio) {                   // remap QF priority to Win32 priority
        case 1:
            p = THREAD_PRIORITY_IDLE;
            break;
        case 2:
            p = THREAD_PRIORITY_LOWEST;
            break;
        case 3:
            p = THREAD_PRIORITY_BELOW_NORMAL;
            break;
        case (QF_MAX_ACTIVE - 1):
            p = THREAD_PRIORITY_ABOVE_NORMAL;
            break;
        case QF_MAX_ACTIVE:
            p = THREAD_PRIORITY_HIGHEST;
            break;
        default:
            p = THREAD_PRIORITY_NORMAL;
            break;
    }
    SetThreadPriority(m_thread, p);
}
//............................................................................
void QActive::stop(void) {
    m_running = (uint8_t)0;                    // stop the QActive::run() loop
    CloseHandle(m_osObject);                           // cleanup the OS event
}
//............................................................................
static DWORD WINAPI thread_function(LPVOID me) {         // for CreateThread()
    ((QActive *)me)->run();
    return 0;                                                // return success
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif

