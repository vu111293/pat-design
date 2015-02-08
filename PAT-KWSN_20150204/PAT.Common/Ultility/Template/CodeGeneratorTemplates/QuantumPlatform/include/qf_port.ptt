//////////////////////////////////////////////////////////////////////////////
// Product: QF/C++  port to Win32
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
#ifndef qf_port_h
#define qf_port_h

                                         // Win32 event queue and thread types
#define QF_OS_OBJECT_TYPE           HANDLE
#define QF_EQUEUE_TYPE              QEQueue
#define QF_THREAD_TYPE              HANDLE

                    // The maximum number of active objects in the application
#define QF_MAX_ACTIVE               63
                       // The maximum number of event pools in the application
#define QF_MAX_EPOOL                8
                        // various QF object sizes configuration for this port
#define QF_EVENT_SIZ_SIZE           4
#define QF_EQUEUE_CTR_SIZE          4
#define QF_MPOOL_SIZ_SIZE           4
#define QF_MPOOL_CTR_SIZE           4
#define QF_TIMEEVT_CTR_SIZE         4

                                         // Win32 critical section, see NOTE01
// QF_CRIT_STAT_TYPE not defined
#define QF_CRIT_ENTRY(dummy)        EnterCriticalSection(&QF_win32CritSect_)
#define QF_CRIT_EXIT(dummy)         LeaveCriticalSection(&QF_win32CritSect_)

#include <windows.h>                                              // Win32 API
#include "qep_port.h"                                              // QEP port
#include "qequeue.h"                                // Win32 needs event-queue
#include "qmpool.h"                                 // Win32 needs memory-pool
#include "qf.h"                    // QF platform-independent public interface


//////////////////////////////////////////////////////////////////////////////
// interface used only inside QF, but not in applications

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

                                   // Win32-specific event queue customization
#define QACTIVE_EQUEUE_WAIT_(me_) \
    while ((me_)->m_eQueue.m_frontEvt == (QEvent const *)0) { \
        QF_CRIT_EXIT_(); \
        (void)WaitForSingleObject((me_)->m_osObject, (DWORD)INFINITE); \
        QF_CRIT_ENTRY_(); \
    }

#define QACTIVE_EQUEUE_SIGNAL_(me_) \
    (void)SetEvent((me_)->m_osObject)

#define QACTIVE_EQUEUE_ONEMPTY_(me_) ((void)0)

                                       // Win32-specific event pool operations
#define QF_EPOOL_TYPE_              QMPool
#define QF_EPOOL_INIT_(p_, poolSto_, poolSize_, evtSize_) \
    (p_).init(poolSto_, poolSize_, evtSize_)
#define QF_EPOOL_EVENT_SIZE_(p_)    ((p_).getBlockSize())
#define QF_EPOOL_GET_(p_, e_)       ((e_) = (QEvent *)(p_).get())
#define QF_EPOOL_PUT_(p_, e_)       ((p_).put(e_))

extern CRITICAL_SECTION QF_win32CritSect_;           // Win32 critical section

void QF_setTickRate(uint32_t ticksPerSec);              // set clock tick rate

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif

//////////////////////////////////////////////////////////////////////////////
//
// NOTE01:
// QF, like all real-time frameworks, needs to execute certain sections of
// code indivisibly to avoid data corruption. The most straightforward way of
// protecting such critical sections of code is disabling and enabling
// interrupts, which Win32 does not allow.
//
// This QF port uses therefore a single package-scope Win32 critical section
// object QF_win32CritSect_ to protect all critical sections.
//
// Using the single critical section object for all crtical section guarantees
// that only one thread at a time can execute inside a critical section. This
// prevents race conditions and data corruption.
//
// Please note, however, that the Win32 critical section implementation
// behaves differently than interrupt locking. A common Win32 critical section
// ensures that only one thread at a time can execute a critical section, but
// it does not guarantee that a context switch cannot occur within the
// critical section. In fact, such context switches probably will happen, but
// they should not cause concurrency hazards because the critical section
// eliminates all race conditionis.
//
// Unlinke simply disabling and enabling interrupts, the critical section
// approach is also subject to priority inversions. Various versions of
// Windows handle priority inversions differently, but it seems that most of
// them recognize priority inversions and dynamically adjust the priorities of
// threads to prevent it. Please refer to the MSN articles for more
// information.
//

#endif                                                            // qf_port_h
