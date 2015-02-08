//////////////////////////////////////////////////////////////////////////////
// Product: QF/C++
// Last Updated for Version: 4.3.00
// Date of the Last Update:  Nov 01, 2011
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
//#include "qassert.h"

/// \file
/// \ingroup qf
/// \brief QEQueue::get() implementation.

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

//Q_DEFINE_THIS_MODULE(qeq_get)

//............................................................................
QEvent const *QEQueue::get(void) {
    QEvent const *e;
    QF_CRIT_STAT_

    QF_CRIT_ENTRY_();
    if (m_frontEvt == (QEvent *)0) {                    // is the queue empty?
        e = (QEvent *)0;                    // no event available at this time
    }
    else {
        e = m_frontEvt;

        if (m_nFree != m_end) {          // any events in the the ring buffer?
            m_frontEvt = m_ring[m_tail];         // remove event from the tail
            if (m_tail == (QEQueueCtr)0) {           // need to wrap the tail?
                m_tail = m_end;                                 // wrap around
            }
            --m_tail;

            ++m_nFree;               // one more free event in the ring buffer

            QS_BEGIN_NOCRIT_(QS_QF_EQUEUE_GET, QS::eqObj_, this)
                QS_TIME_();                                       // timestamp
                QS_SIG_(e->sig);                   // the signal of this event
                QS_OBJ_(this);                            // this queue object
                QS_U8_(EVT_POOL_ID(e));            // the pool Id of the event
                QS_U8_(EVT_REF_CTR(e));          // the ref count of the event
                QS_EQC_(m_nFree);                    // number of free entries
            QS_END_NOCRIT_()
        }
        else {
            m_frontEvt = (QEvent *)0;               // the queue becomes empty

            QS_BEGIN_NOCRIT_(QS_QF_EQUEUE_GET_LAST, QS::eqObj_, this)
                QS_TIME_();                                       // timestamp
                QS_SIG_(e->sig);                   // the signal of this event
                QS_OBJ_(this);                            // this queue object
                QS_U8_(EVT_POOL_ID(e));            // the pool Id of the event
                QS_U8_(EVT_REF_CTR(e));          // the ref count of the event
            QS_END_NOCRIT_()
        }
    }
    QF_CRIT_EXIT_();
    return e;
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif
